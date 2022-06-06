namespace DialogFramework.Application.Extensions;

public static class DialogServiceExtensions
{
    public static IDialog Continue(this IDialogService instance, IDialog context)
        => instance.Continue(context, new[] { new DialogPartResult(context.CurrentPartId, new EmptyDialogPartResultDefinition().Id, new EmptyDialogPartResultValue()) });

    public static IDialog Continue(this IDialogService instance, IDialog context, params IDialogPartResult[] results)
        => instance.Continue(context, results.AsEnumerable());
}
