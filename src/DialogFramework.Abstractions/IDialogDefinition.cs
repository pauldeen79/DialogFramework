namespace DialogFramework.Abstractions;

public interface IDialogDefinition
{
    IDialogMetadata Metadata { get; }
    IReadOnlyCollection<IDialogPart> Parts { get; }
    IErrorDialogPart ErrorPart { get; }
    IAbortedDialogPart AbortedPart { get; }
    ICompletedDialogPart CompletedPart { get; }
    IReadOnlyCollection<IDialogPartGroup> PartGroups { get; }

    IEnumerable<IDialogPartResult> ReplaceAnswers(IEnumerable<IDialogPartResult> existingAnswers,
                                                  IEnumerable<IDialogPartResultAnswer> newAnswers,
                                                  IDialogDefinitionIdentifier dialogId,
                                                  IDialogPartIdentifier partId);

    Result<IEnumerable<IDialogPartResult>> ResetPartResultsByPartId(IEnumerable<IDialogPartResult> existingAnswers,
                                                                    IDialogPartIdentifier partId);
    Result CanNavigateTo(IDialogPartIdentifier currentPartId,
                         IDialogPartIdentifier navigateToPartId,
                         IEnumerable<IDialogPartResult> existingAnswers);

    Result<IDialogPart> GetFirstPart();

    Result<IDialogPart> GetNextPart(IDialog dialog, IEnumerable<IDialogPartResultAnswer> answers);

    Result<IDialogPart> GetPartById(IDialogPartIdentifier id);
}
