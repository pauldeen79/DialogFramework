namespace DialogFramework.Domain.DialogParts;

public partial record DecisionDialogPart : DialogPart
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

    public override IDialogPartBuilder CreateBuilder() => new DecisionDialogPartBuilder(this);
}
