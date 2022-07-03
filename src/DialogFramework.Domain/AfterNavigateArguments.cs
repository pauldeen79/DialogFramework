namespace DialogFramework.Domain;

public partial record AfterNavigateArguments : NavigateArgumentsBase, IAfterNavigateArguments
{
    public AfterNavigateArguments(IDialog dialog, IConditionEvaluator evaluator, DialogAction action)
        : base(dialog, evaluator, action)
    {
    }
}
