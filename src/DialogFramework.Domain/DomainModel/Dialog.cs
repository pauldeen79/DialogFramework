namespace DialogFramework.Domain.DomainModel;

public partial record Dialog
{
    public IEnumerable<IDialogPartResult> ReplaceAnswers(IEnumerable<IDialogPartResult> existingDialogPartResults,
                                          IEnumerable<IDialogPartResult> newDialogPartResults)
    {
        // Decision: By default, only the results from the requested part are replaced.
        // In case this you need to remove other results as well (for example because a decision or navigation outcome is different), then you need to override this method.
        var dialogPartIds = newDialogPartResults
            .GroupBy(x => x.DialogPartId)
            .Select(x => x.Key)
            .ToArray();
        return existingDialogPartResults
            .Where(x => !dialogPartIds.Contains(x.DialogPartId))
            .Concat(newDialogPartResults);
    }

    public IEnumerable<IDialogPartResult> ResetDialogPartResultByPart(IEnumerable<IDialogPartResult> existingDialogPartResults,
                                                                      IDialogPart currentPart)
    {
        // Decision: By default, only remove the results from the requested part.
        // In case this you need to remove other results as well (for example because a decision or navigation outcome is different), then you need to override this method.
        return existingDialogPartResults.Where(x => x.DialogPartId != currentPart.Id);
    }

    public bool CanNavigateTo(IDialogPart currentPart,
                              IDialogPart navigateToPart,
                              IEnumerable<IDialogPartResult> existingDialogPartResults)
    {
        // Decision: By default, you can navigate to either the current part, or any part you have already visited.
        // In case you want to allow navigate forward to parts that are not visited yet, then you need to override this method.
        return currentPart.Id == navigateToPart.Id || existingDialogPartResults.Any(x => x.DialogPartId == navigateToPart.Id);
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
                                   IDialogPart currentPart,
                                   IConditionEvaluator conditionEvaluator,
                                   IEnumerable<IDialogPartResult> providedAnswers)
    {
        // first perform validation
        var error = currentPart.Validate(context, this, conditionEvaluator, providedAnswers);
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

    public IDialogPart GetPartById(IDialogContext context,
                                   string id,
                                   IConditionEvaluator conditionEvaluator)
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
                dialogPart = GetPartById(context, nextPartId, conditionEvaluator);
            }
            else if (dialogPart is INavigationDialogPart navigationDialogPart)
            {
                dialogPart = GetPartById(context, navigationDialogPart.GetNextPartId(context), conditionEvaluator);
            }
            else
            {
                break;
            }
        }

        return dialogPart;
    }
}
