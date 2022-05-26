namespace DialogFramework.Abstractions.DomainModel;

public interface IDecision
{
    IReadOnlyCollection<ICondition> Conditions { get; }
    string NextPartId { get; }
}
