namespace DialogFramework.Abstractions.DomainModel;

public interface IDialogValidationResult
{
    string ErrorMessage { get; }
    ValueCollection<string> DialogPartResultIds { get; }
}
