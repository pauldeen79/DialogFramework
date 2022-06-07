namespace DialogFramework.Domain.Extensions;

public static class DialogPartExtensions
{
    public static IDialogPart? Validate(this IDialogPart part,
                                        IDialog dialog,
                                        IDialogDefinition dialogDefinition,
                                        IEnumerable<IDialogPartResult> providedAnswers)
        => part is IQuestionDialogPart questionDialogPart
            ? questionDialogPart.Validate(dialog, dialogDefinition, providedAnswers)
            : null;

    public static IDialogPartGroup? GetGroup(this IDialogPart part)
        => part is IGroupedDialogPart groupedDialogPart
            ? groupedDialogPart.Group
            : null;

    public static IDialogPartGroupIdentifier? GetGroupId(this IDialogPart part)
        => part.GetGroup()?.Id;

    public static IEnumerable<IDialogValidationResult> GetValidationResults(this IDialogPart part)
        => (part as IQuestionDialogPart)?.ValidationErrors ?? Enumerable.Empty<IDialogValidationResult>();
}
