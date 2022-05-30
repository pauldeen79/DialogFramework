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

    internal static IDialogPartGroup? GetGroup(this IDialogPart part)
        => part is IGroupedDialogPart groupedDialogPart
            ? groupedDialogPart.Group
            : null;

    internal static DialogPartGroupIdentifierBuilder? GetGroupIdBuilder(this IDialogPart part)
    {
        var group = part.GetGroup();
        return group == null
            ? null
            : new DialogPartGroupIdentifierBuilder(group.Id);
    }

    public static IEnumerable<IDialogValidationResult> GetValidationResults(this IDialogPart part)
        => (part as IQuestionDialogPart)?.ValidationErrors ?? Enumerable.Empty<IDialogValidationResult>();
}
