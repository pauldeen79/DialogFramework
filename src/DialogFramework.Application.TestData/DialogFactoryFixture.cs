namespace DialogFramework.Application.TestData;

[ExcludeFromCodeCoverage]
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

    public bool CanCreate(IDialogDefinition dialogDefinition, IEnumerable<IDialogPartResult> dialogPartResults)
        => _canCreateDelegate(dialogDefinition);

    public IDialog Create(IDialogDefinition dialogDefinition, IEnumerable<IDialogPartResult> dialogPartResults)
        => _createDelegate(dialogDefinition);
}
