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

    public bool CanCreate(IDialogDefinition dialogDefinition)
        => _canCreateDelegate(dialogDefinition);

    public IDialog Create(IDialogDefinition dialogDefinition)
        => _createDelegate(dialogDefinition);
}
