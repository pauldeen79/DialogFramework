namespace DialogFramework.Domain.Extensions;

internal static class DecisionDialogPartExtensions
{
    internal static DialogPartIdentifierBuilder? GetDefaultNextPartIdBuilder(this IDecisionDialogPart instance)
    {
        var decisionDialogPart = instance as DecisionDialogPart;
        if (decisionDialogPart == null) return null;
        if (decisionDialogPart.DefaultNextPartId == null) return null;
        return new DialogPartIdentifierBuilder(decisionDialogPart.DefaultNextPartId);
    }

    internal static IEnumerable<DecisionBuilder> GetDecisionBuilders(this IDecisionDialogPart instance)
    {
        var decisionDialogPart = instance as DecisionDialogPart;
        if (decisionDialogPart == null) return Enumerable.Empty<DecisionBuilder>();

        return decisionDialogPart.Decisions.Select(x => new DecisionBuilder(x));
    }
}
