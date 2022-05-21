namespace DialogFramework.Core.Tests.Fixtures;

internal record SportsTypeDecisionDialogPart : DecisionDialogPart
{
    public SportsTypeDecisionDialogPart(string id) : base(id)
    {
    }

    public override string GetNextPartId(IDialogContext context)
        => context.GetDialogPartResultsByPart(context.CurrentDialog.Parts.Single(x => x.Id == "SportsTypes")).Any(x => x.Value is not EmptyDialogPartResultValue)
            ? "Healthy"
            : "Unhealthy";
}
