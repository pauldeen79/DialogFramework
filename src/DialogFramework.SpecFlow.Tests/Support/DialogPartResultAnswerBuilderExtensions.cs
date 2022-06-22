namespace DialogFramework.SpecFlow.Tests.Support;

public static class DialogPartResultAnswerBuilderExtensions
{
    public static DialogPartResultAnswerBuilder EvaluateExpressions(this DialogPartResultAnswerBuilder instance)
    {
        instance.Value.Value = ValueExpression.Evaluate(instance.Value.Value);
        return instance;
    }
}
