namespace DialogFramework.Abstractions.DomainModel;

public interface IDialog
{
    IDialogMetadata Metadata { get; }
    ValueCollection<IDialogPart> Parts { get; }
    IErrorDialogPart ErrorPart { get; }
    IAbortedDialogPart AbortedPart { get; }
    ICompletedDialogPart CompletedPart { get; }
    ValueCollection<IDialogPartGroup> PartGroups { get; }
    IEnumerable<IDialogPartResult> ReplaceAnswers(IEnumerable<IDialogPartResult> existingDialogPartResults,
                                                  IEnumerable<IDialogPartResult> newDialogPartResults);
    IEnumerable<IDialogPartResult> ResetDialogPartResultByPart(IEnumerable<IDialogPartResult> existingDialogPartResults, IDialogPart currentPart);
    bool CanNavigateTo(IDialogPart currentPart, IDialogPart navigateToPart, IEnumerable<IDialogPartResult> existingDialogPartResults);
}
