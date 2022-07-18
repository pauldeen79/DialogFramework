namespace DialogFramework.Domain.DialogParts;

public partial record CompletedDialogPart : DialogPart
{
    public override DialogState GetState() => DialogState.Completed;
    public override IDialogPartBuilder CreateBuilder() => new CompletedDialogPartBuilder(this);
    public override bool SupportsReset() => false;
}
