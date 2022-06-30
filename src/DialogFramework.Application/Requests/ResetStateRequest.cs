namespace DialogFramework.Application.Requests;

public record ResetStateRequest
{
    public IDialog Dialog { get; }
    public IDialogDefinitionIdentifier DialogDefinitionIdentifier { get; }

    public ResetStateRequest(IDialog dialog)
        : this(dialog, dialog.CurrentDialogIdentifier)
    {
    }

    public ResetStateRequest(IDialog dialog,
                             IDialogDefinitionIdentifier dialogDefinitionIdentifier)
    {
        Dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
        DialogDefinitionIdentifier = dialogDefinitionIdentifier ?? throw new ArgumentNullException(nameof(dialogDefinitionIdentifier));
    }
}
