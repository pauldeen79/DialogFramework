namespace DialogFramework.Domain.DomainModel.DialogParts;

public partial record ErrorDialogPart
{
    public DialogState GetState() => DialogState.ErrorOccured;
}
