namespace DialogFramework.Domain.DomainModel.DialogParts;

public partial record DecisionDialogPart : IConditionEvaluatorContainer
{
    public IConditionEvaluator ConditionEvaluator { get; set; } = new DummyConditionEvaluator();

    public string GetNextPartId(IDialogContext context, IDialog dialog)
        => Decisions.FirstOrDefault(x => ConditionEvaluator.Evaluate(context, x.Conditions))?.NextPartId
        ?? DefaultNextPartId.WhenNullOrEmpty(() => throw new NotSupportedException("There is no decision for this path"));

    public DialogState GetState() => DialogState.InProgress;

    private sealed class DummyConditionEvaluator : IConditionEvaluator
    {
        public bool Evaluate(object? context, IEnumerable<ICondition> conditions)
            => throw new NotSupportedException("ConditionEvaluator property was not set, this is probably a bug in the DialogService");
    }
}
