namespace DialogFramework.Core.Extensions;

internal static class DialogPartExtensions
{
    internal static IDialogPart? Validate(this IDialogPart part, IEnumerable<IDialogPartResult> providedAnswers)
        => part is IQuestionDialogPart questionDialogPart
            ? questionDialogPart.Validate(providedAnswers)
            : null;

    internal static IDialogPart ProcessDecisions(this IDialogPart dialogPart,
                                                 IDialogContext context)
    => dialogPart is IDecisionDialogPart decisionDialogPart
        ? ProcessDecisions(decisionDialogPart.GetNextPart(context), context)
        : dialogPart;
}
