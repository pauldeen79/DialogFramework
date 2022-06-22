namespace DialogFramework.SpecFlow.Tests.Support;

public static class DialogPartResultAnswerBuilderExtensions
{
    public static DialogPartResultAnswerBuilder EvaluateExpressions(this DialogPartResultAnswerBuilder instance)
    {
        instance.Value.Value = TableValueHelpers.EvaluateExpressions(instance.Value.Value);
        return instance;
    }
}
