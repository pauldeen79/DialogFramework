namespace DialogFramework.Domain.DialogParts;

public abstract record DialogPartBase
{
    public virtual void AfterNavigate(IAfterNavigateArguments args)
    {
        // Method intentionally left empty.
    }

    public virtual void BeforeNavigate(IBeforeNavigateArguments args)
    {
        // Method intentionally left empty.
    }
}
