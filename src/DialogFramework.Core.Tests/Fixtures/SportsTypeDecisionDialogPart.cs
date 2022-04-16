namespace DialogFramework.Core.Tests.Fixtures;

internal record SportsTypeDecisionDialogPart : DecisionDialogPart
{
    public SportsTypeDecisionDialogPart(string id) : base(id)
    {
    }

    public override IDialogPart GetNextPart(IDialogContext context)
    {
        var nextId = context.GetDialogPartResultsByPart(context.CurrentDialog.Parts.Single(x => x.Id == "SportsTypes")).Any()
            ? "Unhealthy"
            : "Healthy";
        return context.CurrentDialog.Parts.Single(x => x.Id == nextId);
    }
}
