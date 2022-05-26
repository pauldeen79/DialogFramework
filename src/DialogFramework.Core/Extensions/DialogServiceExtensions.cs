namespace DialogFramework.Core.Extensions;

public static class DialogServiceExtensions
{
    public static IDialogContext Continue(this IDialogService instance, IDialogContext context)
        => instance.Continue(context, new[] { new DialogPartResult(context.CurrentPart.Id, new EmptyDialogPartResultDefinition().Id, new EmptyDialogPartResultValue()) });

    public static IDialogContext Continue(this IDialogService instance, IDialogContext context, params IDialogPartResult[] results)
        => instance.Continue(context, results.AsEnumerable());
}
