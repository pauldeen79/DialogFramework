namespace DialogFramework.Abstractions.DialogParts;

public interface IErrorDialogPart : IDialogPart
{
    string ErrorMessage { get; }
    IError? Error { get; }
}
