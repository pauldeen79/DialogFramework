﻿namespace DialogFramework.Abstractions;

public interface IDialogDefinition
{
    IDialogMetadata Metadata { get; }
    IReadOnlyCollection<IDialogPart> Parts { get; }
    IErrorDialogPart ErrorPart { get; }
    IAbortedDialogPart AbortedPart { get; }
    ICompletedDialogPart CompletedPart { get; }
    IReadOnlyCollection<IDialogPartGroup> PartGroups { get; }

    IEnumerable<IDialogPartResult> ReplaceAnswers(IEnumerable<IDialogPartResult> existingPartResults,
                                                  IEnumerable<IDialogPartResult> newPartResults);

    bool CanResetPartResultsByPartId(IDialogPartIdentifier partId);

    IEnumerable<IDialogPartResult> ResetPartResultsByPartId(IEnumerable<IDialogPartResult> existingPartResults,
                                                            IDialogPartIdentifier partId);
    bool CanNavigateTo(IDialogPartIdentifier currentPartId,
                       IDialogPartIdentifier navigateToPartId,
                       IEnumerable<IDialogPartResult> existingPartResults);

    bool CanStart(IDialog context, IConditionEvaluator conditionEvaluator);

    IDialogPart GetFirstPart(IDialog context, IConditionEvaluator conditionEvaluator);

    IDialogPart GetNextPart(IDialog context,
                            IConditionEvaluator conditionEvaluator,
                            IEnumerable<IDialogPartResult> providedResults);

    IDialogPart GetPartById(IDialogPartIdentifier id);
}
