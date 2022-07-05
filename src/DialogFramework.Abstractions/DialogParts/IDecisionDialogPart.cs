namespace DialogFramework.Abstractions.DialogParts;

public interface IDecisionDialogPart : IDialogPart
{
    Result<IDialogPartIdentifier> GetNextPartId(IDialog dialog,
                                                IDialogDefinition dialogDefinition,
                                                IConditionEvaluator conditionEvaluator);
}
