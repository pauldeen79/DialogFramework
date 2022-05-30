namespace DialogFramework.Abstractions.DialogParts;

public interface IMessageDialogPart : IGroupedDialogPart
{
    string Message { get; }
}
