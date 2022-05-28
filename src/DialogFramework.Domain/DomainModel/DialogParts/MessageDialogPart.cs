namespace DialogFramework.Domain.DomainModel.DialogParts;

public partial record MessageDialogPart
{
    public DialogState GetState() => DialogState.InProgress;
}
