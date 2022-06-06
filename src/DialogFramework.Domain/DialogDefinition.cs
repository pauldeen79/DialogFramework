﻿namespace DialogFramework.Domain;

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

    public bool CanResetPartResultsByPartId(IDialogPartIdentifier partId) => GetPartById(partId) is IQuestionDialogPart;

    public IEnumerable<IDialogPartResult> ResetPartResultsByPartId(IEnumerable<IDialogPartResult> existingPartResults,
                                                                   IDialogPartIdentifier partId)
    {
        if (!CanResetPartResultsByPartId(partId))
        {
            throw new InvalidOperationException("Cannot reset part results");
        }
        // Decision: By default, only remove the results from the requested part.
        // In case this you need to remove other results as well (for example because a decision or navigation outcome is different), then you need to override this method.
        return existingPartResults.Where(x => !Equals(x.DialogPartId, partId));
    }

    public bool CanNavigateTo(IDialogPartIdentifier currentPartId,
                              IDialogPartIdentifier navigateToPartId,
                              IEnumerable<IDialogPartResult> existingPartResults)
    {
        // Decision: By default, you can navigate to either the current part, or any part you have already visited.
        // In case you want to allow navigate forward to parts that are not visited yet, then you need to override this method.
        return Equals(currentPartId, navigateToPartId) || existingPartResults.Any(x => Equals(x.DialogPartId, navigateToPartId));
    }

    public bool CanStart(IDialog dialog, IConditionEvaluator conditionEvaluator)
    {
        var firstPart = Parts.FirstOrDefault() ?? CompletedPart;
        if (firstPart == null)
        {
            return false;
        }

        return TryGetDynamicResult(firstPart, dialog, conditionEvaluator) != null;
    }

    public IDialogPart GetFirstPart(IDialog dialog, IConditionEvaluator conditionEvaluator)
    {
        var firstPart = Parts.FirstOrDefault() ?? CompletedPart;

        return GetDynamicResult(firstPart, dialog, conditionEvaluator);
    }

    public IDialogPart GetNextPart(IDialog dialog,
                                   IConditionEvaluator conditionEvaluator,
                                   IEnumerable<IDialogPartResult> providedResults)
    {
        // first perform validation
        var currentPart = GetPartById(dialog.CurrentPartId);
        var error = currentPart.Validate(dialog, this, providedResults);
        if (error != null)
        {
            return error;
        }

        // if validation succeeds, then get the next part
        var parts = Parts
            .Select((part, index) => new { Index = index, Part = part })
            .ToArray();
        var currentPartWithIndex = parts
            .FirstOrDefault(p => Equals(p.Part.Id, currentPart.Id));
        var nextPartWithIndex = parts
            .Where(p => currentPartWithIndex != null && p.Index > currentPartWithIndex.Index)
            .OrderBy(p => p.Index)
            .FirstOrDefault();
        if (nextPartWithIndex == null)
        {
            // there is no next part, so get the completed part
            return GetDynamicResult(CompletedPart, dialog, conditionEvaluator);
        }

        return GetDynamicResult(nextPartWithIndex.Part, dialog, conditionEvaluator);
    }

    public IDialogPart GetPartById(IDialogPartIdentifier id)
    {
        if (Equals(id, AbortedPart.Id)) return AbortedPart;
        if (Equals(id, CompletedPart.Id)) return CompletedPart;
        if (Equals(id, ErrorPart.Id)) return ErrorPart;
        var parts = Parts.Where(x => Equals(x.Id, id)).ToArray();
        if (parts.Length == 1)
        {
            return parts[0];
        }
        throw new InvalidOperationException($"Dialog does not have a part with id [{id}]");
    }

    private IDialogPart GetDynamicResult(IDialogPart dialogPart,
                                         IDialog dialog,
                                         IConditionEvaluator conditionEvaluator)
    {
        while (true)
        {
            if (dialogPart is IDecisionDialogPart decisionDialogPart)
            {
                var nextPartId = decisionDialogPart.GetNextPartId(dialog, this, conditionEvaluator);
                dialogPart = GetPartById(nextPartId);
            }
            else if (dialogPart is INavigationDialogPart navigationDialogPart)
            {
                dialogPart = GetPartById(navigationDialogPart.GetNextPartId(dialog));
            }
            else
            {
                break;
            }
        }

        return dialogPart;
    }

    private IDialogPart? TryGetDynamicResult(IDialogPart dialogPart,
                                             IDialog dialog,
                                             IConditionEvaluator conditionEvaluator)
    {
        IDialogPart? result = dialogPart;
        while (true)
        {
            if (result is IDecisionDialogPart decisionDialogPart)
            {
                var nextPartId = decisionDialogPart.GetNextPartId(dialog, this, conditionEvaluator);
                result = TryGetPartById(nextPartId);
            }
            else if (result is INavigationDialogPart navigationDialogPart)
            {
                result = TryGetPartById(navigationDialogPart.GetNextPartId(dialog));
            }
            else
            {
                break;
            }
        }

        return result;
    }

    private IDialogPart? TryGetPartById(IDialogPartIdentifier id)
    {
        if (Equals(id, AbortedPart.Id)) return AbortedPart;
        if (Equals(id, CompletedPart.Id)) return CompletedPart;
        if (Equals(id, ErrorPart.Id)) return ErrorPart;
        var parts = Parts.Where(x => Equals(x.Id, id)).ToArray();
        if (parts.Length == 1)
        {
            return parts[0];
        }
        if (parts.Length > 1)
        {
            return null;
        }
        return null;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var duplicateIds = new[] { AbortedPart.Id, CompletedPart.Id, ErrorPart.Id }
            .Concat(Parts.Select(x => x.Id))
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();

        if (duplicateIds.Any())
        {
            yield return new ValidationResult($"DialogPart Ids should be unique. Non unique ids: {string.Join(", ", duplicateIds.Select(x => x.ToString()))}");
        }
    }
}