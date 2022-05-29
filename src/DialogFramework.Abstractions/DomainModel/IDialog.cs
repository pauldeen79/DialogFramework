namespace DialogFramework.Abstractions.DomainModel;

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

    IEnumerable<IDialogPartResult> ResetDialogPartResultByPart(IEnumerable<IDialogPartResult> existingPartResults,
                                                               IDialogPart part);
    bool CanNavigateTo(IDialogPart currentPart,
                       IDialogPart navigateToPart,
                       IEnumerable<IDialogPartResult> existingPartResults);

    IDialogPart GetFirstPart(IDialogContext context, IConditionEvaluator conditionEvaluator);

    IDialogPart GetNextPart(IDialogContext context,
                            IDialogPart currentPart,
                            IConditionEvaluator conditionEvaluator,
                            IEnumerable<IDialogPartResult> providedResults);

    IDialogPart GetPartById(IDialogContext context,
                            string id,
                            IConditionEvaluator conditionEvaluator);
}
