namespace DialogFramework.Abstractions.DialogParts;

public interface IAbortedDialogPart : IDialogPart
{
    string Message { get; }
}
