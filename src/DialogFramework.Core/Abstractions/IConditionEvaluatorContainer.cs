namespace DialogFramework.Core.Abstractions;

public interface IConditionEvaluatorContainer
{
    IConditionEvaluator ConditionEvaluator { get; set; }
}
