namespace DialogFramework.Application.TestData;

public class DialogContextFactoryFixture : IDialogContextFactory
{
    private readonly Func<IDialogDefinition, IDialogContext> _createDelegate;
    private readonly Func<IDialogDefinition, bool> _canCreateDelegate;

    public DialogContextFactoryFixture(Func<IDialogDefinition, bool> canCreateDelegate,
                                       Func<IDialogDefinition, IDialogContext> createDelegate)
    {
        _canCreateDelegate = canCreateDelegate;
        _createDelegate = createDelegate;
    }

    public bool CanCreate(IDialogDefinition dialog)
        => _canCreateDelegate(dialog);

    public IDialogContext Create(IDialogDefinition dialog)
        => _createDelegate(dialog);
}
