namespace DialogFramework.Domain.DialogParts;

public partial record MessageDialogPart
{
    public DialogState GetState() => DialogState.InProgress;
    public IDialogPartBuilder CreateBuilder() => new MessageDialogPartBuilder(this);
    public bool SupportsReset => false;
}
