namespace DialogFramework.Core.Tests.Fixtures;

internal record NavigationDialogPartFixture : NavigationDialogPart
{
    private readonly Func<IDialogContext, IDialogPart> _getNextPartDelegate;

    public NavigationDialogPartFixture(string id,
                                       Func<IDialogContext, IDialogPart> getNextPartDelegate)
        : base(id) => _getNextPartDelegate = getNextPartDelegate;

    public override IDialogPart GetNextPart(IDialogContext context)
        => _getNextPartDelegate(context);
}
