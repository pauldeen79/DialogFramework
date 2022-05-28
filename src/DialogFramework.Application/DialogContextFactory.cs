namespace DialogFramework.Application;

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
            Enumerable.Empty<IDialogPartResult>()
        );

    private sealed class EmptyDialogPart : IDialogPart
    {
        public string Id => "Empty";
        public DialogState GetState() => DialogState.Initial;
    }
}
