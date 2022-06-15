namespace DialogFramework.Domain.Extensions;

public static class DialogPartExtensions
{
    public static Result Validate(this IDialogPart part,
                                  IDialog dialog,
                                  IDialogDefinition definition,
                                  IEnumerable<IDialogPartResult> results)
        => part is IQuestionDialogPart questionDialogPart
            ? questionDialogPart.Validate(dialog, definition, results)
            : Result.Success();

    public static IDialogPartGroup? GetGroup(this IDialogPart part)
        => part is IGroupedDialogPart groupedDialogPart
            ? groupedDialogPart.Group
            : null;

    public static IDialogPartGroupIdentifier? GetGroupId(this IDialogPart part)
        => part.GetGroup()?.Id;
}
