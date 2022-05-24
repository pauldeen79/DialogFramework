namespace DialogFramework.Abstractions.DomainModel.DialogParts;

public interface IDecisionDialogPart : IDialogPart
{
    ValueCollection<IDecision> Decisions { get; }
    string GetNextPartId(IDialogContext context, IDialog dialog, IConditionEvaluator evaluator);
}
