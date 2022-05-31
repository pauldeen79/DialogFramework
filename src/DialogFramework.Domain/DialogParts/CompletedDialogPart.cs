namespace DialogFramework.Domain.DialogParts;

public partial record CompletedDialogPart
{
    public DialogState GetState() => DialogState.Completed;
    public IDialogPartBuilder CreateBuilder() => new CompletedDialogPartBuilder(this);
}
