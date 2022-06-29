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

    public Result<IDialog> Create(IDialogDefinition dialogDefinition, IEnumerable<IDialogPartResult> dialogPartResults)
    {
        if (!_canCreateDelegate(dialogDefinition))
        {
            return Result<IDialog>.Error("This error was created by DialogFactoryFixture");
        }

        return Result<IDialog>.Success(_createDelegate(dialogDefinition));
    }
}
