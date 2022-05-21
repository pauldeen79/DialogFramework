namespace DialogFramework.Core.Extensions;

internal static class DialogPartExtensions
{
    internal static IDialogPart? Validate(this IDialogPart part, IDialogContext context, IEnumerable<IDialogPartResult> providedAnswers)
        => part is IQuestionDialogPart questionDialogPart
            ? questionDialogPart.Validate(context, providedAnswers)
            : null;

    internal static IDialogPart ProcessDecisions(this IDialogPart dialogPart, IDialogContext context)
    {
        if (dialogPart is IDecisionDialogPart decisionDialogPart)
        {
            var id = decisionDialogPart.GetNextPartId(context);
            return ProcessDecisions(context.CurrentDialog.GetPartById(id), context);
        }

        return dialogPart;
    }
}
