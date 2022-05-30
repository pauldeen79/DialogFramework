namespace DialogFramework.Application.TestData;

public class DialogContextFactoryFixture : IDialogContextFactory
{
    private readonly Func<IDialog, IDialogContext> _createDelegate;
    private readonly Func<IDialog, bool> _canCreateDelegate;

    public DialogContextFactoryFixture(Func<IDialog, bool> canCreateDelegate,
                                       Func<IDialog, IDialogContext> createDelegate)
    {
        _canCreateDelegate = canCreateDelegate;
        _createDelegate = createDelegate;
    }

    public bool CanCreate(IDialog dialog)
        => _canCreateDelegate(dialog);

    public IDialogContext Create(IDialog dialog)
        => _createDelegate(dialog);
}
