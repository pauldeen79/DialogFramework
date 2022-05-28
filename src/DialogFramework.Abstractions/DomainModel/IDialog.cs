namespace DialogFramework.Abstractions.DomainModel;

public interface IDialog
{
    IDialogMetadata Metadata { get; }
    IReadOnlyCollection<IDialogPart> Parts { get; }
    IErrorDialogPart ErrorPart { get; }
    IAbortedDialogPart AbortedPart { get; }
    ICompletedDialogPart CompletedPart { get; }
    IReadOnlyCollection<IDialogPartGroup> PartGroups { get; }

    IEnumerable<IDialogPartResult> ReplaceAnswers(IEnumerable<IDialogPartResult> existingDialogPartResults,
                                                  IEnumerable<IDialogPartResult> newDialogPartResults);

    IEnumerable<IDialogPartResult> ResetDialogPartResultByPart(IEnumerable<IDialogPartResult> existingDialogPartResults,
                                                               IDialogPart currentPart);
    bool CanNavigateTo(IDialogPart currentPart,
                       IDialogPart navigateToPart,
                       IEnumerable<IDialogPartResult> existingDialogPartResults);

    IDialogPart GetFirstPart(IDialogContext context, IConditionEvaluator conditionEvaluator);

    IDialogPart GetNextPart(IDialogContext context,
                            IDialogPart currentPart,
                            IConditionEvaluator conditionEvaluator,
                            IEnumerable<IDialogPartResult> providedAnswers);

    IDialogPart GetPartById(IDialogContext context,
                            string id,
                            IConditionEvaluator conditionEvaluator);
}
