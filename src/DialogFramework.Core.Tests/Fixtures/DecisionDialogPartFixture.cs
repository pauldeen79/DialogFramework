namespace DialogFramework.Core.Tests.Fixtures;

internal record DecisionDialogPartFixture : DecisionDialogPart
{
    private readonly Func<IDialogContext, IDialog, IConditionEvaluator, string> _getNextPartIdDelegate;

    public DecisionDialogPartFixture(string id, Func<IDialogContext, IDialog, IConditionEvaluator, string> getNextPartIdDelegate)
        : base(id, Enumerable.Empty<IDecision>()) => _getNextPartIdDelegate = getNextPartIdDelegate;

    public override string GetNextPartId(IDialogContext context, IDialog dialog, IConditionEvaluator evaluator)
        => _getNextPartIdDelegate(context, dialog, evaluator);
}
