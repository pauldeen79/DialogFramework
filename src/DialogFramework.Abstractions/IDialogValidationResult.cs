namespace DialogFramework.Abstractions;

public interface IDialogValidationResult
{
    string ErrorMessage { get; }
    IReadOnlyCollection<string> DialogPartResultIds { get; }
}
