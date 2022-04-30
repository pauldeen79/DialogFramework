namespace DialogFramework.Core.Tests.Fixtures;

internal record NavigationDialogPartFixture : NavigationDialogPart
{
    private readonly Func<IDialogContext, string> _getNextPartIdDelegate;

    public NavigationDialogPartFixture(string id, Func<IDialogContext, string> getNextPartIdDelegate)
        : base(id) => _getNextPartIdDelegate = getNextPartIdDelegate;

    public override string GetNextPartId(IDialogContext context)
        => _getNextPartIdDelegate(context);
}
