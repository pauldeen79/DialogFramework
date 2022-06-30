namespace DialogFramework.Application.Requests;

public record NavigateRequest
{
    public IDialog Dialog { get; }
    public IDialogDefinitionIdentifier DialogDefinitionIdentifier { get; }
    public IDialogPartIdentifier NavigateToPartId { get; }

    public NavigateRequest(IDialog dialog,
                           IDialogPartIdentifier navigateToPartId)
        : this(dialog, dialog?.CurrentDialogIdentifier!, navigateToPartId)
    {
    }

    public NavigateRequest(IDialog dialog,
                           IDialogDefinitionIdentifier dialogDefinitionIdentifier,
                           IDialogPartIdentifier navigateToPartId)
    {
        Dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
        DialogDefinitionIdentifier = dialogDefinitionIdentifier ?? throw new ArgumentNullException(nameof(dialogDefinitionIdentifier));
        NavigateToPartId = navigateToPartId ?? throw new ArgumentNullException(nameof(navigateToPartId));
    }
}
