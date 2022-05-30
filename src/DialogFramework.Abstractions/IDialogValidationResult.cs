namespace DialogFramework.Abstractions;

public interface IDialogValidationResult
{
    string ErrorMessage { get; }
    IReadOnlyCollection<IDialogPartResultIdentifier> DialogPartResultIds { get; }
}
