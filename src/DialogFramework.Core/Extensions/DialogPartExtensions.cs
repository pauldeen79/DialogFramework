namespace DialogFramework.Core.Extensions;

internal static class DialogPartExtensions
{
    internal static IDialogPart? Validate(this IDialogPart part,
                                          IDialogContext context,
                                          IDialog dialog,
                                          IEnumerable<IDialogPartResult> providedAnswers)
        => part is IQuestionDialogPart questionDialogPart
            ? questionDialogPart.Validate(context, dialog, providedAnswers)
            : null;

    internal static DialogState GetState(this IDialogPart part)
    {
        if (part is IAbortedDialogPart) return DialogState.Aborted;
        if (part is ICompletedDialogPart) return DialogState.Completed;
        if (part is IErrorDialogPart) return DialogState.ErrorOccured;
        return DialogState.InProgress;
    }

    internal static IDialogPartGroup? GetGroup(this IDialogPart part)
        => part is IGroupedDialogPart groupedDialogPart
            ? groupedDialogPart.Group
            : null;
}
