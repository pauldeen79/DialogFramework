namespace DialogFramework.Abstractions;

public interface IDialog
{
    IDialogMetadata Metadata { get; }
    IReadOnlyCollection<IDialogPart> Parts { get; }
    IErrorDialogPart ErrorPart { get; }
    IAbortedDialogPart AbortedPart { get; }
    ICompletedDialogPart CompletedPart { get; }
    IReadOnlyCollection<IDialogPartGroup> PartGroups { get; }

    IEnumerable<IDialogPartResult> ReplaceAnswers(IEnumerable<IDialogPartResult> existingPartResults,
                                                  IEnumerable<IDialogPartResult> newPartResults);

    bool CanResetPartResultsByPartId(string partId);

    IEnumerable<IDialogPartResult> ResetPartResultsByPartId(IEnumerable<IDialogPartResult> existingPartResults,
                                                            string partId);
    bool CanNavigateTo(string currentPartId,
                       string navigateToPartId,
                       IEnumerable<IDialogPartResult> existingPartResults);

    IDialogPart GetFirstPart(IDialogContext context, IConditionEvaluator conditionEvaluator);
    IDialogPart GetNextPart(IDialogContext context,
                            IConditionEvaluator conditionEvaluator,
                            IEnumerable<IDialogPartResult> providedResults);

    IDialogPart GetPartById(string id);
}
