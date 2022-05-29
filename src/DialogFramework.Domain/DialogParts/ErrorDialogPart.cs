namespace DialogFramework.Domain.DialogParts;

public partial record ErrorDialogPart
{
    public DialogState GetState() => DialogState.ErrorOccured;
}
