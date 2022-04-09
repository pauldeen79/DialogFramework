namespace DialogFramework.Abstractions.DomainModel;

public interface IDialog
{
    string Id { get; }
    string Title { get; }
    string Version { get; }
    ValueCollection<IDialogPart> Parts { get; }
    IErrorDialogPart ErrorPart { get; }
    IAbortedDialogPart AbortedPart { get; }
    ICompletedDialogPart CompletedPart { get; }
    ValueCollection<IDialogPartGroup> PartGroups { get; }
}
