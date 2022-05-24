namespace DialogFramework.Abstractions.DomainModel;

public interface IDecision
{
    ValueCollection<ICondition> Conditions { get; }
    string NextPartId { get; }
}
