namespace DialogFramework.Domain.DialogParts;

public partial record MessageDialogPart
{
    public override DialogState GetState() => DialogState.InProgress;
    public override IDialogPartBuilder CreateBuilder() => new MessageDialogPartBuilder(this);
    public override bool SupportsReset() => false;
}
