namespace DialogFramework.Domain.DialogParts;

public partial record RedirectDialogPart
{
    public override IDialogPartBuilder CreateBuilder() => new RedirectDialogPartBuilder(this);
}
