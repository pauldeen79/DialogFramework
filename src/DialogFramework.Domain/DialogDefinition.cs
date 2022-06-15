namespace DialogFramework.Domain;

public partial record DialogDefinition : IValidatableObject
{
    public IEnumerable<IDialogPartResult> ReplaceAnswers(IEnumerable<IDialogPartResult> existingPartResults,
                                                         IEnumerable<IDialogPartResult> newPartResults)
    {
        // Decision: By default, only the results from the requested part are replaced.
        // In case this you need to remove other results as well (for example because a decision or navigation outcome is different), then you need to override this method.
        var dialogPartIds = newPartResults
            .GroupBy(x => x.DialogPartId)
            .Select(x => x.Key)
            .ToArray();
        return existingPartResults
            .Where(x => !dialogPartIds.Any(y => Equals(y, x.DialogPartId)))
            .Concat(newPartResults);
    }

    public Result<IEnumerable<IDialogPartResult>> ResetPartResultsByPartId(IEnumerable<IDialogPartResult> existingPartResults,
                                                                           IDialogPartIdentifier partId)
    {
        var partByIdResult = GetPartById(partId);
        if (!partByIdResult.IsSuccessful())
        {
            return Result<IEnumerable<IDialogPartResult>>.FromExistingResult(partByIdResult);
        }
        if (!partByIdResult.Value!.SupportsReset())
        {
            // Part does not support reset (probably a informational part like message, error, aborted or completed)
            return Result<IEnumerable<IDialogPartResult>>.Invalid("The specified part cannot be reset");
        }

        // Decision: By default, only remove the results from the requested part.
        // In case this you need to remove other results as well (for example because a decision or navigation outcome is different), then you need to override this method.
        return Result<IEnumerable<IDialogPartResult>>.Success(existingPartResults.Where(x => !Equals(x.DialogPartId, partId)));
    }

    public Result CanNavigateTo(IDialogPartIdentifier currentPartId,
                                IDialogPartIdentifier navigateToPartId,
                                IEnumerable<IDialogPartResult> existingPartResults)
    {
        // Decision: By default, you can navigate to either the current part, or any part you have already visited.
        // In case you want to allow navigate forward to parts that are not visited yet, then you need to override this method.
        if (!(Equals(currentPartId, navigateToPartId) || existingPartResults.Any(x => Equals(x.DialogPartId, navigateToPartId))))
        {
            // Part has not been visited yet
            return Result.Invalid("Cannot navigate to the specified part");
        }

        return Result.Success();
    }

    public Result<IDialogPart> GetFirstPart(IDialog dialog, IConditionEvaluator evaluator)
        => GetDynamicResult(Parts.FirstOrDefault() ?? CompletedPart, dialog, evaluator);

    public Result<IDialogPart> GetNextPart(IDialog dialog, IConditionEvaluator evaluator, IEnumerable<IDialogPartResult> results)
    {
        // first perform validation
        var currentPartResult = GetPartById(dialog.CurrentPartId);
        if (!currentPartResult.IsSuccessful())
        {
            return currentPartResult;
        }
        var validationResult = currentPartResult.Value!.Validate(dialog, this, results);
        if (!validationResult.IsSuccessful())
        {
            return Result<IDialogPart>.FromExistingResult(validationResult);
        }

        // if validation succeeds, then get the next part
        var parts = Parts
            .Select((part, index) => new { Index = index, Part = part })
            .ToArray();
        var currentPartWithIndex = parts.FirstOrDefault(p => Equals(p.Part.Id, currentPartResult.Value!.Id));
        var nextPartWithIndex = parts
            .Where(p => currentPartWithIndex != null && p.Index > currentPartWithIndex.Index)
            .OrderBy(p => p.Index)
            .FirstOrDefault();
        if (nextPartWithIndex == null)
        {
            // there is no next part, so get the completed part
            return GetDynamicResult(CompletedPart, dialog, evaluator);
        }

        return GetDynamicResult(nextPartWithIndex.Part, dialog, evaluator);
    }

    public Result<IDialogPart> GetPartById(IDialogPartIdentifier id)
    {
        var part = this.GetAllParts().FirstOrDefault(x => Equals(x.Id, id));
        if (part == null)
        {
            return Result<IDialogPart>.NotFound("Dialog does not have a part with id [{id}]");
        }
        return Result<IDialogPart>.Success(part);
    }

    private Result<IDialogPart> GetDynamicResult(IDialogPart dialogPart, IDialog dialog, IConditionEvaluator evaluator)
    {
        while (true)
        {
            if (dialogPart is IDecisionDialogPart decisionDialogPart)
            {
                var nextPartId = decisionDialogPart.GetNextPartId(dialog, this, evaluator);
                var partByIdResult = GetPartById(nextPartId);
                if (!partByIdResult.IsSuccessful())
                {
                    return partByIdResult;
                }
                dialogPart = partByIdResult.Value!;
            }
            else if (dialogPart is INavigationDialogPart navigationDialogPart)
            {
                var partByIdResult = GetPartById(navigationDialogPart.GetNextPartId(dialog));
                if (!partByIdResult.IsSuccessful())
                {
                    return partByIdResult;
                }
                dialogPart = partByIdResult.Value!;
            }
            else
            {
                break;
            }
        }

        return Result<IDialogPart>.Success(dialogPart);
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var duplicateIds = this.GetAllParts()
            .Select(x => x.Id)
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();

        if (duplicateIds.Any())
        {
            yield return new ValidationResult($"Dialog part ids should be unique. Duplicate ids: {string.Join(", ", duplicateIds.Select(x => x.ToString()))}");
        }
    }
}
