namespace DialogFramework.Core.DomainModel;

public class DialogValidationResult : IDialogValidationResult
{
    public DialogValidationResult(string errorMessage) : this(errorMessage, new ValueCollection<string>())
    {
    }

    public DialogValidationResult(string errorMessage, ValueCollection<string> dialogPartResultIds)
    {
        ErrorMessage = errorMessage;
        DialogPartResultIds = dialogPartResultIds;
    }

    public string ErrorMessage { get; }
    public ValueCollection<string> DialogPartResultIds { get; }
}
