namespace DialogFramework.Application.TestData;

public class DialogFactoryFixture : IDialogFactory
{
    private readonly Func<IDialogDefinition, IDialog> _createDelegate;
    private readonly Func<IDialogDefinition, bool> _canCreateDelegate;

    public DialogFactoryFixture(Func<IDialogDefinition, bool> canCreateDelegate,
                                       Func<IDialogDefinition, IDialog> createDelegate)
    {
        _canCreateDelegate = canCreateDelegate;
        _createDelegate = createDelegate;
    }

    public bool CanCreate(IDialogDefinition dialog)
        => _canCreateDelegate(dialog);

    public IDialog Create(IDialogDefinition dialog)
        => _createDelegate(dialog);
}
