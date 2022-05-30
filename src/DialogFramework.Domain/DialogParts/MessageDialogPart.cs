namespace DialogFramework.Domain.DialogParts;

public partial record MessageDialogPart
{
    public DialogState GetState() => DialogState.InProgress;
}
