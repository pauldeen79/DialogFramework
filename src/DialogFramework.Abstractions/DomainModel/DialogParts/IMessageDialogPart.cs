namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IMessageDialogPart : IDialogPart
{
    string Message { get; }
    IDialogPartGroup Group { get; }
}
