namespace DialogFramework.Domain;

public partial record BeforeNavigateArguments : NavigateArgumentsBase, IBeforeNavigateArguments
{
    public BeforeNavigateArguments(IDialog dialog, IDialogDefinition definition, IConditionEvaluator evaluator, DialogAction action)
        : base(dialog, definition, evaluator, action)
    {
    }
}
