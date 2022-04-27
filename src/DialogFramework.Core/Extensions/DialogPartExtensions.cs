namespace DialogFramework.Core.Extensions;

internal static class DialogPartExtensions
{
    internal static IDialogPart? Validate(this IDialogPart part, IEnumerable<IDialogPartResult> providedAnswers)
        => part is IQuestionDialogPart questionDialogPart
            ? questionDialogPart.Validate(providedAnswers)
            : null;

    internal static IDialogPart ProcessDecisions(this IDialogPart dialogPart, IDialogContext context)
        => dialogPart is IDecisionDialogPart decisionDialogPart
            ? ProcessDecisions(decisionDialogPart.GetNextPart(context), context)
            : dialogPart;

    internal static ResultValueType? GetResultValueType(this IDialogPart dialogPart, IDialogPartResult dialogPartResult)
        => dialogPart is IQuestionDialogPart questionDialogPart
            ? questionDialogPart.Results.FirstOrDefault(x => x.Id == dialogPartResult.Result.Id)?.ValueType
            : null;
}
