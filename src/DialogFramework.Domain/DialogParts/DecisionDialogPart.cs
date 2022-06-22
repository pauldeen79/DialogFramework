namespace DialogFramework.Domain.DialogParts;

public partial record DecisionDialogPart
{
    public IDialogPartIdentifier GetNextPartId(IDialog dialog,
                                               IDialogDefinition dialogDefinition,
                                               IConditionEvaluator conditionEvaluator)
        => Decisions.FirstOrDefault(x => conditionEvaluator.Evaluate(dialog, x.Conditions))?.NextPartId
        ?? DefaultNextPartId ?? throw new NotSupportedException("There is no decision for this path");

    public DialogState GetState() => DialogState.InProgress;
    public IDialogPartBuilder CreateBuilder() => new DecisionDialogPartBuilder(this);
    public bool SupportsReset() => false;
}
