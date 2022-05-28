namespace DialogFramework.Domain.DomainModel.DialogParts;

public partial record DecisionDialogPart
{
    public string GetNextPartId(IDialogContext context, IDialog dialog, IConditionEvaluator conditionEvaluator)
        => Decisions.FirstOrDefault(x => conditionEvaluator.Evaluate(context, x.Conditions))?.NextPartId
        ?? DefaultNextPartId.WhenNullOrEmpty(() => throw new NotSupportedException("There is no decision for this path"));

    public DialogState GetState() => DialogState.InProgress;
}
