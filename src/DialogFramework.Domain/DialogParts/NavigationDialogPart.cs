namespace DialogFramework.Domain.DialogParts;

public partial record NavigationDialogPart
{
    public string GetNextPartId(IDialogContext context) => NavigateToId;

    public DialogState GetState() => DialogState.InProgress;
}
