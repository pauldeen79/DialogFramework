namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogValidationResult
{
    string ErrorMessage { get; }
    IReadOnlyCollection<string> DialogPartResultIds { get; }
}
