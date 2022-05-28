namespace DialogFramework.Domain.Extensions;

public static class DialogPartExtensions
{
    public static IDialogPart? Validate(this IDialogPart part,
                                        IDialogContext context,
                                        IDialog dialog,
                                        IEnumerable<IDialogPartResult> providedAnswers)
        => part is IQuestionDialogPart questionDialogPart
            ? questionDialogPart.Validate(context, dialog, providedAnswers)
            : null;

    public static IDialogPartGroup? GetGroup(this IDialogPart part)
        => part is IGroupedDialogPart groupedDialogPart
            ? groupedDialogPart.Group
            : null;
}
