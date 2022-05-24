namespace DialogFramework.Core.Tests.Fixtures;

internal record AgeDecisionDialogPart : DecisionDialogPart
{
    public AgeDecisionDialogPart(string id) : base(id, Enumerable.Empty<IDecision>())
    {
    }

    public override string GetNextPartId(IDialogContext context, IDialog dialog, IConditionEvaluator evaluator)
        => context.GetDialogPartResultsByPart(dialog.Parts.Single(x => x.Id == "Age"))
                  .Single().DialogPartId == "<10"
            ? "TooYoung"
            : "SportsTypes";
}
