namespace DialogFramework.Abstractions;

public interface IDecision
{
    IReadOnlyCollection<ICondition> Conditions { get; }
    string NextPartId { get; }
}
