namespace DialogFramework.Domain;

public class DialogContextFactory : IDialogContextFactory
{
    public bool CanCreate(IDialog dialog) => dialog is Dialog;

    public IDialogContext Create(IDialog dialog)
        => new DialogContext
        (
            Guid.NewGuid().ToString(),
            dialog.Metadata,
            new EmptyDialogPart(),
            null,
            DialogState.Initial,
            new ValueCollection<IDialogPartResult>(),
            null
        );

    private sealed class EmptyDialogPart : IDialogPart
    {
        public string Id => "Empty";
    }
}
