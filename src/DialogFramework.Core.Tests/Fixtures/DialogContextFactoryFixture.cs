namespace DialogFramework.Core.Tests.Fixtures;

internal class DialogContextFactoryFixture : IDialogContextFactory
{
    private readonly Func<IDialog, IDialogContext> _createDelegate;

    public DialogContextFactoryFixture(Func<IDialog, IDialogContext> createDelegate)
        => _createDelegate = createDelegate;

    public IDialogContext Create(IDialog dialog)
        => _createDelegate(dialog);
}
