namespace DialogFramework.Core.DomainModel.DialogParts;

public abstract record DecisionDialogPart : IDecisionDialogPart
{
    protected DecisionDialogPart(string id, IEnumerable<IDecision> decisions)
    {
        Id = id;
        Decisions = new ValueCollection<IDecision>(decisions);
    }

    public abstract string GetNextPartId(IDialogContext context, IDialog dialog, IConditionEvaluator evaluator);
    public string Id { get; }
    public DialogState State => DialogState.InProgress;
    public ValueCollection<IDecision> Decisions { get; }
}
