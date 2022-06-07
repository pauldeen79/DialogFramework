namespace DialogFramework.Abstractions.DialogParts;

public interface IDecisionDialogPart : IDialogPart
{
    IDialogPartIdentifier GetNextPartId(IDialog dialog,
                                        IDialogDefinition dialogDefinition,
                                        IConditionEvaluator conditionEvaluator);
}
