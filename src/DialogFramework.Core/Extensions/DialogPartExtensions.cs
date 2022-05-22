namespace DialogFramework.Core.Extensions;

internal static class DialogPartExtensions
{
    internal static IDialogPart? Validate(this IDialogPart part,
                                          IDialogContext context,
                                          IEnumerable<IDialogPartResult> providedAnswers)
        => part is IQuestionDialogPart questionDialogPart
            ? questionDialogPart.Validate(context, providedAnswers)
            : null;

    internal static IDialogPart ProcessDecisions(this IDialogPart dialogPart,
                                                 IDialogContext context,
                                                 IDialogRepository dialogRepository)
    {
        if (dialogPart is IDecisionDialogPart decisionDialogPart)
        {
            var dialog = dialogRepository.GetDialog(context.CurrentDialogIdentifier);
            if (dialog == null)
            {
                throw new InvalidOperationException($"Unknown dialog: Id [{context.CurrentDialogIdentifier.Id}], Version [{context.CurrentDialogIdentifier.Version}]");
            }

            var id = decisionDialogPart.GetNextPartId(context, dialog);
            return ProcessDecisions(dialog.GetPartById(id), context, dialogRepository);
        }

        return dialogPart;
    }
}
