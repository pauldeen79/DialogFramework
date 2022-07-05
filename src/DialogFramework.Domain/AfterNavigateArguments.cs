namespace DialogFramework.Domain;

public partial record AfterNavigateArguments : NavigateArgumentsBase, IAfterNavigateArguments
{
    public AfterNavigateArguments(IDialog dialog, IDialogDefinition definition, IConditionEvaluator evaluator, DialogAction action)
        : base(dialog, definition, evaluator, action)
    {
    }
}
