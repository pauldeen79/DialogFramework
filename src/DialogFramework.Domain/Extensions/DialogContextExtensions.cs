namespace DialogFramework.Domain.Extensions;

public static class DialogContextExtensions
{
    public static IEnumerable<IDialogPartResult> GetDialogPartResultsByPartIdentifier(this IDialogContext context,
                                                                                      IDialogPartIdentifier dialogPartIdentifier)
        => context.Results.Where(x => Equals(x.DialogPartId, dialogPartIdentifier));
}
