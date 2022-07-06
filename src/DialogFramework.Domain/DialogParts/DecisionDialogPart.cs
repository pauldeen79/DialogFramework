namespace DialogFramework.Domain.DialogParts;

public partial record DecisionDialogPart : DialogPartBase
{
    public override void BeforeNavigate(IBeforeNavigateArguments args)
    {
        args.CancelStateUpdate();

        var partId = Decisions.FirstOrDefault(x => args.ConditionEvaluator.Evaluate(args, x.Conditions))?.NextPartId
            ?? DefaultNextPartId;

        if (partId == null)
        {
            args.Result = Result<IDialogPart>.Error("No next dialog part supplied");
        }
        else
        {
            var partByIdResult = args.DialogDefinition.GetPartById(partId);
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

    public DialogState GetState() => DialogState.InProgress;
    public IDialogPartBuilder CreateBuilder() => new DecisionDialogPartBuilder(this);
    public bool SupportsReset() => false;
}
