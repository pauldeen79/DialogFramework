namespace DialogFramework.Domain.DialogParts;

public abstract partial record DialogPart
{
    public virtual Result<IDialogPart>? AfterNavigate(IAfterNavigateArguments args)
        => default;

    public virtual Result<IDialogPart>? BeforeNavigate(IBeforeNavigateArguments args)
        => default;

    public abstract IDialogPartBuilder CreateBuilder();
    public virtual DialogState GetState() => DialogState.InProgress;
    public virtual bool SupportsReset() => false;
}
