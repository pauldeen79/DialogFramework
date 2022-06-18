namespace DialogFramework.Domain.DialogParts;

public partial record ErrorDialogPart
{
    public DialogState GetState() => DialogState.ErrorOccured;
    public IDialogPartBuilder CreateBuilder() => new ErrorDialogPartBuilder(this);
    public bool SupportsReset => false;
}
