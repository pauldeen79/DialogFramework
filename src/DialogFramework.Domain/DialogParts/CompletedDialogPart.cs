namespace DialogFramework.Domain.DialogParts;

public partial record CompletedDialogPart
{
    public DialogState GetState() => DialogState.Completed;
}
