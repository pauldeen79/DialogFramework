namespace DialogFramework.Core.Tests.Fixtures;

internal record DecisionDialogPartFixture : DecisionDialogPart
{
    private readonly Func<IDialogContext, IDialogPart> _getNextPartDelegate;

    public DecisionDialogPartFixture(string id,
                                     Func<IDialogContext, IDialogPart> getNextPartDelegate)
        : base(id) => _getNextPartDelegate = getNextPartDelegate;

    public override IDialogPart GetNextPart(IDialogContext context)
        => _getNextPartDelegate(context);
}
