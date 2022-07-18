namespace DialogFramework.Domain.DialogParts;

public partial record MessageDialogPart
{
    public override IDialogPartBuilder CreateBuilder() => new MessageDialogPartBuilder(this);
}
