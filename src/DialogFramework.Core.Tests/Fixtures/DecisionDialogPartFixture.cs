namespace DialogFramework.Core.Tests.Fixtures;

internal record DecisionDialogPartFixture : DecisionDialogPart
{
    private readonly Func<IDialogContext, string> _getNextPartIdDelegate;

    public DecisionDialogPartFixture(string id, Func<IDialogContext, string> getNextPartIdDelegate)
        : base(id) => _getNextPartIdDelegate = getNextPartIdDelegate;

    public override string GetNextPartId(IDialogContext context)
        => _getNextPartIdDelegate(context);
}
