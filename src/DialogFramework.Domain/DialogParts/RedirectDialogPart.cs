namespace DialogFramework.Domain.DialogParts;

public partial record RedirectDialogPart
{
    public override DialogState GetState() => DialogState.Completed;
    public override IDialogPartBuilder CreateBuilder() => new RedirectDialogPartBuilder(this);
    public override bool SupportsReset() => false;
}
