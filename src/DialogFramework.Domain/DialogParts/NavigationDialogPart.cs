namespace DialogFramework.Domain.DialogParts;

public partial record NavigationDialogPart
{
    public override DialogState GetState() => DialogState.InProgress;
    public override IDialogPartBuilder CreateBuilder() => new NavigationDialogPartBuilder(this);
    public override bool SupportsReset() => false;

    public override Result<IDialogPart>? BeforeNavigate(IBeforeNavigateArguments args)
        => args.DialogDefinition.GetPartById(NavigateToId);
}
