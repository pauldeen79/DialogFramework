namespace DialogFramework.Domain.DialogParts;

public partial record MessageDialogPart
{
    public DialogState GetState() => DialogState.InProgress;
    public IDialogPartBuilder CreateBuilder() => new MessageDialogPartBuilder(this);
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
