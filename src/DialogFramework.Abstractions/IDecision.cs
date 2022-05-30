namespace DialogFramework.Abstractions;

public interface IDecision
{
    IReadOnlyCollection<ICondition> Conditions { get; }
    IDialogPartIdentifier NextPartId { get; }
}
