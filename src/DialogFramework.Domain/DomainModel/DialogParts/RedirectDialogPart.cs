namespace DialogFramework.Domain.DomainModel.DialogParts;

public partial record RedirectDialogPart
{
    public DialogState GetState() => DialogState.InProgress;
}
