namespace DialogFramework.Domain.Extensions;

public static class DialogExtensions
{
    public static IEnumerable<IDialogPartResult> GetDialogPartResultsByPartIdentifier(this IDialog context,
                                                                                      IDialogPartIdentifier dialogPartIdentifier)
        => context.Results.Where(x => Equals(x.DialogPartId, dialogPartIdentifier));

    public static void Error(this IDialog context, IDialogDefinition dialog, params IError[] errorMessages)
        => context.Error(dialog, errorMessages.AsEnumerable());
}
