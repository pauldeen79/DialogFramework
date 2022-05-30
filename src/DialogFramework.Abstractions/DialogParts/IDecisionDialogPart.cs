namespace DialogFramework.Abstractions.DialogParts;

public interface IDecisionDialogPart : IDialogPart
{
    IDialogPartIdentifier GetNextPartId(IDialogContext context, IDialog dialog, IConditionEvaluator conditionEvaluator);
}
