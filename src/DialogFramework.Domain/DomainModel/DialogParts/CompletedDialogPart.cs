namespace DialogFramework.Domain.DomainModel.DialogParts;

public partial record CompletedDialogPart
{
    public DialogState GetState() => DialogState.Completed;
}
