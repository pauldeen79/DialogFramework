namespace DialogFramework.Domain.DialogParts;

public partial record RedirectDialogPart : DialogPartBase
{
    public DialogState GetState() => DialogState.Completed;
    public IDialogPartBuilder CreateBuilder() => new RedirectDialogPartBuilder(this);
    public bool SupportsReset() => false;
}
