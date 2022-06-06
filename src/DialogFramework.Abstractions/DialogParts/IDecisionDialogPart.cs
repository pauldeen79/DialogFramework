namespace DialogFramework.Abstractions.DialogParts;

public interface IDecisionDialogPart : IDialogPart
{
    IDialogPartIdentifier GetNextPartId(IDialog context, IDialogDefinition dialog, IConditionEvaluator conditionEvaluator);
}
