namespace DialogFramework.Domain.Abstractions;

public interface IConditionEvaluatorContainer
{
    IConditionEvaluator ConditionEvaluator { get; set; }
}
