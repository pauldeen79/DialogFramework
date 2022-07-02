namespace DialogFramework.Domain.DialogParts;

public partial record ErrorDialogPart
{
    public DialogState GetState() => DialogState.ErrorOccured;
    public IDialogPartBuilder CreateBuilder() => new ErrorDialogPartBuilder(this);
    public bool SupportsReset() => false;

    public void AfterNavigate(IAfterNavigateArguments args)
    {
        // Method intentionally left empty.
    }

    public void BeforeNavigate(IBeforeNavigateArguments args)
    {
        // Method intentionally left empty.
    }
}
