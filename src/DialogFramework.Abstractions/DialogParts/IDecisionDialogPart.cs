namespace DialogFramework.Abstractions.DialogParts;

public interface IDecisionDialogPart : IDialogPart
{
    IDialogPartIdentifier GetNextPartId(IDialogContext context, IDialogDefinition dialog, IConditionEvaluator conditionEvaluator);
}
