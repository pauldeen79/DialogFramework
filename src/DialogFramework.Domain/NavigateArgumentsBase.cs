namespace DialogFramework.Domain;

public abstract record NavigateArgumentsBase : INavigateArguments
{
    public IDialogIdentifier CurrentDialogId { get; }
    public IDialogDefinitionIdentifier CurrentDialogIdentifier { get; set; }
    public IDialogPartIdentifier CurrentPartId { get; set; }
    public IDialogPartGroupIdentifier? CurrentGroupId { get; set; }
    public DialogState CurrentState { get; set; }
    public string? ErrorMessage { get; set; }
    public Result? Result { get; set; }
    public bool UpdateState { get; private set; } = true;

    public DialogAction Action { get; }
    public IDialogDefinition DialogDefinition { get; }
    public IConditionEvaluator ConditionEvaluator { get; }

    protected IDialog Dialog { get; }

    protected NavigateArgumentsBase(IDialog dialog, IDialogDefinition definition, IConditionEvaluator evaluator, DialogAction action)
    {
        if (dialog == null)
        {
            throw new ArgumentNullException(nameof(dialog));
        }
        if (evaluator == null)
        {
            throw new ArgumentNullException(nameof(evaluator));
        }
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        CurrentDialogId = dialog.Id;
        CurrentDialogIdentifier = dialog.CurrentDialogIdentifier;
        CurrentGroupId = dialog.CurrentGroupId;
        CurrentPartId = dialog.CurrentPartId;
        CurrentState = dialog.CurrentState;
        ErrorMessage = dialog.ErrorMessage;
        DialogDefinition = definition;
        ConditionEvaluator = evaluator;
        Action = action;
        Dialog = dialog;
    }

    public void AddProperty(IProperty property)
        => Dialog.AddProperty(property);

    public void CancelStateUpdate()
        => UpdateState = false;
}
