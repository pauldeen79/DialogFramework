namespace DialogFramework.Domain;

public partial record BeforeNavigateArguments : NavigateArgumentsBase, IBeforeNavigateArguments
{
    public bool UpdateState { get; private set; } = true;

    public BeforeNavigateArguments(IDialog dialog, IDialogDefinition definition, IConditionEvaluator evaluator, DialogAction action)
        : base(dialog, definition, evaluator, action)
    {
    }

    public void CancelStateUpdate()
        => UpdateState = false;
}
