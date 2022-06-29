namespace DialogFramework.Application.Extensions;

public static class DialogApplicationServiceExtensions
{
    public static Result<IDialog> Continue(this IDialogApplicationService instance, IDialog dialog)
        => instance.Continue(dialog, new[] { new DialogPartResultAnswer(new EmptyDialogPartResultDefinition().Id, new DialogPartResultValueAnswer(null)) });

    public static Result<IDialog> Continue(this IDialogApplicationService instance, IDialog dialog, params IDialogPartResultAnswer[] results)
        => instance.Continue(dialog, results.AsEnumerable());

    public static Result<IDialog> Start(this IDialogApplicationService instance, IDialogDefinitionIdentifier dialogDefinitionIdentifier)
        => instance.Start(dialogDefinitionIdentifier, Enumerable.Empty<IDialogPartResult>());

    public static Result<IDialog> NavigateTo(this IDialogApplicationService instance, IDialog dialog, IDialogPartIdentifier navigateToPartId)
        => instance.NavigateTo(dialog, dialog.CurrentDialogIdentifier, navigateToPartId);

    public static Result<IDialog> ResetCurrentState(this IDialogApplicationService instance, IDialog dialog)
        => instance.ResetCurrentState(dialog, dialog.CurrentDialogIdentifier);
}
