namespace DialogFramework.Domain.DomainModel.DialogParts;

public partial record DecisionDialogPart : IConditionEvaluatorContainer
{
    public IConditionEvaluator ConditionEvaluator { get; set; } = new DummyConditionEvaluator();

    public string GetNextPartId(IDialogContext context, IDialog dialog)
    {
        var ctx = new Tuple<IDialogContext, IDialog>(context, dialog);
        return Decisions.FirstOrDefault(x => ConditionEvaluator.Evaluate(ctx, x.Conditions))?.NextPartId
            ?? DefaultNextPartId.WhenNullOrEmpty(() => throw new NotSupportedException("There is no decision for this path"));
    }

    private sealed class DummyConditionEvaluator : IConditionEvaluator
    {
        public bool Evaluate(object? context, IEnumerable<ICondition> conditions)
            => throw new NotSupportedException("ConditionEvaluator property was not set, this is probably a bug in the DialogService");
    }
}
