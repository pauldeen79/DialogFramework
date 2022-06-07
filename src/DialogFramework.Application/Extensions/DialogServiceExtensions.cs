namespace DialogFramework.Application.Extensions;

public static class DialogServiceExtensions
{
    public static IDialog Continue(this IDialogService instance, IDialog dialog)
        => instance.Continue(dialog, new[] { new DialogPartResult(dialog.CurrentPartId, new EmptyDialogPartResultDefinition().Id, new EmptyDialogPartResultValue()) });

    public static IDialog Continue(this IDialogService instance, IDialog dialog, params IDialogPartResult[] results)
        => instance.Continue(dialog, results.AsEnumerable());
}
