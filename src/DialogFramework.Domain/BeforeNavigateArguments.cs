namespace DialogFramework.Domain;

public partial record BeforeNavigateArguments
{
    public Result? Result { get; private set; }
    public bool UpdateState { get; private set; } = true;

    private readonly IDialog _dialog;

    public BeforeNavigateArguments(IDialog dialog, IConditionEvaluator evaluator, DialogAction action)
    {
        if (dialog == null)
        {
            throw new ArgumentNullException(nameof(dialog));
        }
        if (evaluator == null)
        {
            throw new ArgumentNullException(nameof(evaluator));
        }

        CurrentDialogId = dialog.Id;
        CurrentDialogIdentifier = dialog.CurrentDialogIdentifier;
        CurrentGroupId = dialog.CurrentGroupId;
        CurrentPartId = dialog.CurrentPartId;
        CurrentState = dialog.CurrentState;
        ErrorMessage = dialog.ErrorMessage;
        ConditionEvaluator = evaluator;
        Action = action;
        _dialog = dialog;
    }

    public void SetResult(Result result)
        => Result = result;

    public void AddProperty(IProperty property)
        => _dialog.AddProperty(property);

    public void CancelStateUpdate()
        => UpdateState = false;
}
