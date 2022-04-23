namespace DialogFramework.Abstractions.DomainModel;

public interface IDialog
{
    IDialogMetadata Metadata { get; }
    ValueCollection<IDialogPart> Parts { get; }
    IErrorDialogPart ErrorPart { get; }
    IAbortedDialogPart AbortedPart { get; }
    ICompletedDialogPart CompletedPart { get; }
    ValueCollection<IDialogPartGroup> PartGroups { get; }
}
