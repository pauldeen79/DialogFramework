namespace DialogFramework.Domain.DialogParts;

public abstract record DialogPartBase
{
    public virtual Result<IDialogPart>? AfterNavigate(IAfterNavigateArguments args)
        => default;

    public virtual Result<IDialogPart>? BeforeNavigate(IBeforeNavigateArguments args)
        => default;
}
