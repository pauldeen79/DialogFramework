namespace DialogFramework.Domain.DialogParts;

public partial record CompletedDialogPart
{
    public DialogState GetState() => DialogState.Completed;
    public IDialogPartBuilder CreateBuilder() => new CompletedDialogPartBuilder(this);
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
