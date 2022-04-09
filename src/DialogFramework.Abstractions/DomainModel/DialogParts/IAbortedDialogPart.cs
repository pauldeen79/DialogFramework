namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IAbortedDialogPart : IDialogPart
{
    string Message { get; }
}
