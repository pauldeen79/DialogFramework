namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface ICompletedDialogPart : IDialogPart
{
    string Message { get; }
    IDialogPartGroup Group { get; }
}
