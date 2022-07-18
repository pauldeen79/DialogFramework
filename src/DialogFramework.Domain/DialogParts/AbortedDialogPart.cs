namespace DialogFramework.Domain.DialogParts;

public partial record AbortedDialogPart
{
    public override DialogState GetState() => DialogState.Aborted;
    public override IDialogPartBuilder CreateBuilder() => new AbortedDialogPartBuilder(this);
}
