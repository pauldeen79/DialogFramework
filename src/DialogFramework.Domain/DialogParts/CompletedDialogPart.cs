namespace DialogFramework.Domain.DialogParts;

public partial record CompletedDialogPart : DialogPartBase
{
    public DialogState GetState() => DialogState.Completed;
    public IDialogPartBuilder CreateBuilder() => new CompletedDialogPartBuilder(this);
    public bool SupportsReset() => false;
}
