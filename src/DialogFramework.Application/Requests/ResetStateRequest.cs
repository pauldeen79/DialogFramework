namespace DialogFramework.Application.Requests;

public record ResetStateRequest : IRequest<Result<IDialog>>
{
    public IDialog Dialog { get; }
    public IDialogDefinitionIdentifier DialogDefinitionIdentifier { get; }
    public IDialogPartIdentifier DialogPartIdentifier { get; }

    public ResetStateRequest(IDialog dialog)
        : this(dialog, dialog?.CurrentDialogId!, dialog?.CurrentPartId!)
    {
    }

    public ResetStateRequest(IDialog dialog,
                             IDialogDefinitionIdentifier dialogDefinitionIdentifier,
                             IDialogPartIdentifier dialogPartIdentifier)
    {
        Dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
        DialogDefinitionIdentifier = dialogDefinitionIdentifier ?? throw new ArgumentNullException(nameof(dialogDefinitionIdentifier));
        DialogPartIdentifier = dialogPartIdentifier ?? throw new ArgumentNullException(nameof(dialogPartIdentifier));
    }
}
