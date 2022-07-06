namespace DialogFramework.Domain.DialogParts;

public partial record NavigationDialogPart : DialogPartBase
{
    public DialogState GetState() => DialogState.InProgress;
    public IDialogPartBuilder CreateBuilder() => new NavigationDialogPartBuilder(this);
    public bool SupportsReset() => false;

    public override void BeforeNavigate(IBeforeNavigateArguments args)
    {
        args.CancelStateUpdate();

        var partByIdResult = args.DialogDefinition.GetPartById(NavigateToId);
        if (!partByIdResult.IsSuccessful())
        {
            args.Result = partByIdResult;
        }
        else
        {
            var part = partByIdResult.GetValueOrThrow();
            args.CurrentPartId = part.Id;
            args.CurrentGroupId = part.GetGroupId();
            args.CurrentState = part.GetState();
        }
    }
}
