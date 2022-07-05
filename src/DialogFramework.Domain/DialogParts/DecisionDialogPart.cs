namespace DialogFramework.Domain.DialogParts;

public partial record DecisionDialogPart : DialogPartBase
{
    public Result<IDialogPartIdentifier> GetNextPartId(IDialog dialog,
                                                       IDialogDefinition dialogDefinition,
                                                       IConditionEvaluator conditionEvaluator)
    {
        var part = Decisions.FirstOrDefault(x => conditionEvaluator.Evaluate(dialog, x.Conditions))?.NextPartId
            ?? DefaultNextPartId;

        if (part == null)
        {
            return Result<IDialogPartIdentifier>.Error("No next dialog part supplied");
        }

        return Result<IDialogPartIdentifier>.Success(part);
    }

    public DialogState GetState() => DialogState.InProgress;
    public IDialogPartBuilder CreateBuilder() => new DecisionDialogPartBuilder(this);
    public bool SupportsReset() => false;
}
