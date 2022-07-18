namespace DialogFramework.Domain.DialogParts;

public partial record ErrorDialogPart
{
    public override DialogState GetState() => DialogState.ErrorOccured;
    public override IDialogPartBuilder CreateBuilder() => new ErrorDialogPartBuilder(this);
}
