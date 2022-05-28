namespace DialogFramework.Domain.DomainModel.DialogParts;

public partial record AbortedDialogPart
{
    public DialogState GetState() => DialogState.Aborted;
}
