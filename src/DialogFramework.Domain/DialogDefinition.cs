namespace DialogFramework.Domain;

public partial record DialogDefinition : IValidatableObject
{
    public IEnumerable<IDialogPartResult> ReplaceAnswers(IEnumerable<IDialogPartResult> existingPartResults,
                                                         IEnumerable<IDialogPartResultAnswer> newPartResults,
                                                         IDialogDefinitionIdentifier dialogId,
                                                         IDialogPartIdentifier dialogPartId)
    {
        // Decision: By default, only the results from the requested part are replaced.
        // In case this you need to remove other results as well (for example because a decision or navigation outcome is different), then you need to override this method.
        return existingPartResults
            .Where(x => !DialogIdEquals(dialogId, x.DialogId) || !Equals(dialogPartId, x.DialogPartId))
            .Concat(newPartResults.Select(x => new DialogPartResult(dialogId, dialogPartId, x.ResultId, x.Value)));
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
        return Result<IEnumerable<IDialogPartResult>>.Success(existingPartResults.Where(x => !DialogIdEquals(x.DialogId, Metadata) || !Equals(x.DialogPartId, partId)));
    }

    public Result CanNavigateTo(IDialogPartIdentifier currentPartId,
                                IDialogPartIdentifier navigateToPartId,
                                IEnumerable<IDialogPartResult> existingPartResults)
    {
        // Decision: By default, you can navigate to either the current part, or any part you have already visited.
        // In case you want to allow navigate forward to parts that are not visited yet, then you need to override this method.
        if (!(Equals(currentPartId, navigateToPartId) || existingPartResults.Any(x => DialogIdEquals(x.DialogId, Metadata) && Equals(x.DialogPartId, navigateToPartId))))
        {
            // Part has not been visited yet
            return Result.Invalid("Cannot navigate to the specified part");
        }

        return Result.Success();
    }

    public Result<IDialogPart> GetFirstPart()
        => Result<IDialogPart>.Success(Parts.FirstOrDefault() ?? CompletedPart);

    public Result<IDialogPart> GetNextPart(IDialog dialog, IEnumerable<IDialogPartResultAnswer> results)
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
            return Result<IDialogPart>.Success(CompletedPart);
        }

        return Result<IDialogPart>.Success(nextPartWithIndex.Part);
    }

    public Result<IDialogPart> GetPartById(IDialogPartIdentifier id)
    {
        var part = this.GetAllParts().FirstOrDefault(x => Equals(x.Id, id));
        if (part == null)
        {
            return Result<IDialogPart>.NotFound($"Dialog does not have a part with id [{id}]");
        }
        return Result<IDialogPart>.Success(part);
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

    private bool DialogIdEquals(IDialogDefinitionIdentifier dialogId1, IDialogDefinitionIdentifier dialogId2)
    {
        // Cast both id's to same type. Needed if one of the two is metadata and other is definition identifier.
        var id1 = new DialogDefinitionIdentifierBuilder(dialogId1).Build();
        var id2 = new DialogDefinitionIdentifierBuilder(dialogId2).Build();
        return Equals(id1, id2);
    }
}
