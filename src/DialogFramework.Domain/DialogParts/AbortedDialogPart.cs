namespace DialogFramework.Domain.DialogParts;

public partial record AbortedDialogPart
{
    public DialogState GetState() => DialogState.Aborted;
    public IDialogPartBuilder CreateBuilder() => new AbortedDialogPartBuilder(this);
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
