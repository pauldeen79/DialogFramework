namespace DialogFramework.Core;

public class DialogContextFactory : IDialogContextFactory
{
    public IDialogContext Create(IDialog dialog)
        => new DialogContext(dialog);
}
