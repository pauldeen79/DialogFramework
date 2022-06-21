namespace DialogFramework.Application.Extensions;

public static class DialogServiceExtensions
{
    public static Result<IDialog> Continue(this IDialogApplicationService instance, IDialog dialog)
        => instance.Continue(dialog, new[] { new DialogPartResultAnswer(new EmptyDialogPartResultDefinition().Id, new DialogPartResultValueAnswer(null)) });

    public static Result<IDialog> Continue(this IDialogApplicationService instance, IDialog dialog, params IDialogPartResultAnswer[] results)
        => instance.Continue(dialog, results.AsEnumerable());
}
