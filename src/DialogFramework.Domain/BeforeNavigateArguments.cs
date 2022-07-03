namespace DialogFramework.Domain;

public partial record BeforeNavigateArguments : NavigateArgumentsBase, IBeforeNavigateArguments
{
    public bool UpdateState { get; private set; } = true;

    public BeforeNavigateArguments(IDialog dialog, IConditionEvaluator evaluator, DialogAction action)
        : base(dialog, evaluator, action)
    {
    }

    public void CancelStateUpdate()
        => UpdateState = false;
}
