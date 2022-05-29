namespace DialogFramework.Application;

public class DialogContextFactory : IDialogContextFactory
{
    public bool CanCreate(IDialog dialog) => dialog is Dialog;

    public IDialogContext Create(IDialog dialog)
        => new DialogContext
        (
            Guid.NewGuid().ToString(),
            dialog.Metadata,
            "Empty",
            null,
            DialogState.Initial,
            Enumerable.Empty<IDialogPartResult>(),
            Enumerable.Empty<IDialogValidationResult>()
        );
}
