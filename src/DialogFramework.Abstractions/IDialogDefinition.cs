namespace DialogFramework.Abstractions;

public interface IDialogDefinition
{
    IDialogMetadata Metadata { get; }
    IReadOnlyCollection<IDialogPart> Parts { get; }
    IErrorDialogPart ErrorPart { get; }
    IAbortedDialogPart AbortedPart { get; }
    ICompletedDialogPart CompletedPart { get; }
    IReadOnlyCollection<IDialogPartGroup> PartGroups { get; }

    IEnumerable<IDialogPartResult> ReplaceAnswers(IEnumerable<IDialogPartResult> existingPartResults,
                                                  IEnumerable<IDialogPartResultAnswer> newPartResults,
                                                  IDialogDefinitionIdentifier dialogId,
                                                  IDialogPartIdentifier dialogPartId);

    Result<IEnumerable<IDialogPartResult>> ResetPartResultsByPartId(IEnumerable<IDialogPartResult> existingPartResults,
                                                                    IDialogPartIdentifier partId);
    Result CanNavigateTo(IDialogPartIdentifier currentPartId,
                         IDialogPartIdentifier navigateToPartId,
                         IEnumerable<IDialogPartResult> existingPartResults);

    Result<IDialogPart> GetFirstPart(IDialog dialog,
                                     IConditionEvaluator evaluator);

    Result<IDialogPart> GetNextPart(IDialog dialog,
                                    IConditionEvaluator evaluator,
                                    IEnumerable<IDialogPartResultAnswer> results);

    Result<IDialogPart> GetPartById(IDialogPartIdentifier id);
}
