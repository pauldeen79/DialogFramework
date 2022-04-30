namespace DialogFramework.Core.Tests.Fixtures;

internal record AgeDecisionDialogPart : DecisionDialogPart
{
    public AgeDecisionDialogPart(string id) : base(id)
    {
    }

    public override IDialogPart GetNextPart(IDialogContext context)
    {
        var nextId = context.GetDialogPartResultsByPart(context.CurrentDialog.Parts.Single(x => x.Id == "Age"))
                            .Single().DialogPartId == "<10"
            ? "TooYoung"
            : "SportsTypes";
        return context.CurrentDialog.Parts.Single(x => x.Id == nextId);
    }
}
