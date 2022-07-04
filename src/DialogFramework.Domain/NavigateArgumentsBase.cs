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

    public DialogAction Action { get; }
    public IConditionEvaluator ConditionEvaluator { get; }

    protected IDialog Dialog { get; }

    protected NavigateArgumentsBase(IDialog dialog, IConditionEvaluator evaluator, DialogAction action)
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
        Dialog = dialog;
    }

    public void AddProperty(IProperty property)
        => Dialog.AddProperty(property);
}
