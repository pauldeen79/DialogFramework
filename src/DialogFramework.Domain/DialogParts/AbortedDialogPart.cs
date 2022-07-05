namespace DialogFramework.Domain.DialogParts;

public partial record AbortedDialogPart : DialogPartBase
{
    public DialogState GetState() => DialogState.Aborted;
    public IDialogPartBuilder CreateBuilder() => new AbortedDialogPartBuilder(this);
    public bool SupportsReset() => false;
}
