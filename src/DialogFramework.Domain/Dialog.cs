namespace DialogFramework.Domain;

public partial record Dialog
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
            .Where(x => !dialogPartIds.Contains(x.DialogPartId))
            .Concat(newPartResults);
    }

    public bool CanResetPartResultsByPartId(string partId) => GetPartById(partId) is IQuestionDialogPart;

    public IEnumerable<IDialogPartResult> ResetPartResultsByPartId(IEnumerable<IDialogPartResult> existingPartResults,
                                                                   string partId)
    {
        // Decision: By default, only remove the results from the requested part.
        // In case this you need to remove other results as well (for example because a decision or navigation outcome is different), then you need to override this method.
        return existingPartResults.Where(x => x.DialogPartId != partId);
    }

    public bool CanNavigateTo(string currentPartId,
                              string navigateToPartId,
                              IEnumerable<IDialogPartResult> existingPartResults)
    {
        // Decision: By default, you can navigate to either the current part, or any part you have already visited.
        // In case you want to allow navigate forward to parts that are not visited yet, then you need to override this method.
        return currentPartId == navigateToPartId || existingPartResults.Any(x => x.DialogPartId == navigateToPartId);
    }

    public IDialogPart GetFirstPart(IDialogContext context, IConditionEvaluator conditionEvaluator)
    {
        var firstPart = Parts.FirstOrDefault();
        if (firstPart == null)
        {
            throw new InvalidOperationException("Could not determine next part. Dialog does not have any parts.");
        }

        return ProcessDecisions(firstPart, context, conditionEvaluator);
    }

    public IDialogPart GetNextPart(IDialogContext context,
                                   IConditionEvaluator conditionEvaluator,
                                   IEnumerable<IDialogPartResult> providedResults)
    {
        // first perform validation
        var currentPart = GetPartById(context.CurrentPartId);
        var error = currentPart.Validate(context, this, providedResults);
        if (error != null)
        {
            return error;
        }

        // if validation succeeds, then get the next part
        var parts = Parts
            .Select((part, index) => new { Index = index, Part = part })
            .ToArray();
        var currentPartWithIndex = parts
            .SingleOrDefault(p => p.Part.Id == currentPart.Id);
        var nextPartWithIndex = parts
            .Where(p => currentPartWithIndex != null && p.Index > currentPartWithIndex.Index)
            .OrderBy(p => p.Index)
            .FirstOrDefault();
        if (nextPartWithIndex == null)
        {
            // there is no next part, so get the completed part
            return ProcessDecisions(CompletedPart, context, conditionEvaluator);
        }

        return ProcessDecisions(nextPartWithIndex.Part, context, conditionEvaluator);
    }

    public IDialogPart GetPartById(string id)
    {
        if (id == AbortedPart.Id) return AbortedPart;
        if (id == CompletedPart.Id) return CompletedPart;
        if (id == ErrorPart.Id) return ErrorPart;
        var parts = Parts.Where(x => x.Id == id).ToArray();
        if (parts.Length == 1)
        {
            return parts[0];
        }
        if (parts.Length > 1)
        {
            throw new InvalidOperationException($"Dialog has multiple parts with id [{id}]");
        }
        throw new InvalidOperationException($"Dialog does not have a part with id [{id}]");
    }

    private IDialogPart ProcessDecisions(IDialogPart dialogPart,
                                         IDialogContext context,
                                         IConditionEvaluator conditionEvaluator)
    {
        while (true)
        {
            if (dialogPart is IDecisionDialogPart decisionDialogPart)
            {
                var nextPartId = decisionDialogPart.GetNextPartId(context, this, conditionEvaluator);
                dialogPart = GetPartById(nextPartId);
            }
            else if (dialogPart is INavigationDialogPart navigationDialogPart)
            {
                dialogPart = GetPartById(navigationDialogPart.GetNextPartId(context));
            }
            else
            {
                break;
            }
        }

        return dialogPart;
    }
}
