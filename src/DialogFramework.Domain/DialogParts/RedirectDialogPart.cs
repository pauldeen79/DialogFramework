namespace DialogFramework.Domain.DialogParts;

public partial record RedirectDialogPart
{
    public DialogState GetState() => DialogState.Completed;
}
