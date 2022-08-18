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

    public Result<IDialog> Create(IDialogDefinition dialogDefinition)
        => Create(dialogDefinition, Enumerable.Empty<IDialogPartResult>(), Enumerable.Empty<IProperty>());

    public Result<IDialog> Create(IDialogDefinition dialogDefinition,
                                  IEnumerable<IDialogPartResult> dialogPartResults)
        => Create(dialogDefinition, dialogPartResults, Enumerable.Empty<IProperty>());

    public Result<IDialog> Create(IDialogDefinition definition,
                                  IEnumerable<IDialogPartResult> results,
                                  IEnumerable<IProperty> properties)
    {
        if (!_canCreateDelegate(definition))
        {
            return Result<IDialog>.Error("This error was created by DialogFactoryFixture");
        }

        return Result<IDialog>.Success(_createDelegate(definition));
    }
}
