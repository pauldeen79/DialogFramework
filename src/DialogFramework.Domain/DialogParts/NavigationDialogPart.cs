namespace DialogFramework.Domain.DialogParts;

public partial record NavigationDialogPart : DialogPartBase
{
    public DialogState GetState() => DialogState.InProgress;
    public IDialogPartBuilder CreateBuilder() => new NavigationDialogPartBuilder(this);
    public bool SupportsReset() => false;

    public override Result<IDialogPart>? BeforeNavigate(IBeforeNavigateArguments args)
        => args.DialogDefinition.GetPartById(NavigateToId);
}
