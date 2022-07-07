namespace DialogFramework.Domain.DialogParts;

public partial record DecisionDialogPart : DialogPartBase
{
    public override Result<IDialogPart>? BeforeNavigate(IBeforeNavigateArguments args)
    {
        var partId = Decisions.FirstOrDefault(x => args.ConditionEvaluator.Evaluate(args, x.Conditions))?.NextPartId
            ?? DefaultNextPartId;

        if (partId == null)
        {
            return Result<IDialogPart>.Error("No next dialog part supplied");
        }
        else
        {
            return args.DialogDefinition.GetPartById(partId);
        }
    }

    public DialogState GetState() => DialogState.InProgress;
    public IDialogPartBuilder CreateBuilder() => new DecisionDialogPartBuilder(this);
    public bool SupportsReset() => false;
}
