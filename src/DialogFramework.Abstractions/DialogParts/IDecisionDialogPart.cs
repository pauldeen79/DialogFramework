namespace DialogFramework.Abstractions.DialogParts;

public interface IDecisionDialogPart : IDialogPart
{
    string GetNextPartId(IDialogContext context, IDialog dialog, IConditionEvaluator conditionEvaluator);
}
