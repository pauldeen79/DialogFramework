namespace DialogFramework.Core.Tests.Fixtures;

internal record DecisionDialogPartFixture : DecisionDialogPart
{
    private readonly Func<IDialogContext, IDialog, string> _getNextPartIdDelegate;

    public DecisionDialogPartFixture(string id, Func<IDialogContext, IDialog, string> getNextPartIdDelegate)
        : base(id) => _getNextPartIdDelegate = getNextPartIdDelegate;

    public override string GetNextPartId(IDialogContext context, IDialog dialog)
        => _getNextPartIdDelegate(context, dialog);
}
