namespace DialogFramework.Domain.DialogParts;

public partial record NavigationDialogPart
{
    public override IDialogPartBuilder CreateBuilder() => new NavigationDialogPartBuilder(this);

    public override Result<IDialogPart>? BeforeNavigate(IBeforeNavigateArguments args)
        => args.Definition.GetPartById(NavigateToId);
}
