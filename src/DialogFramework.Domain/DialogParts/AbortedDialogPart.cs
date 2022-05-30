namespace DialogFramework.Domain.DialogParts;

public partial record AbortedDialogPart
{
    public DialogState GetState() => DialogState.Aborted;
}
