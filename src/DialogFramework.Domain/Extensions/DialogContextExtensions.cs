namespace DialogFramework.Domain.Extensions;

public static class DialogContextExtensions
{
    public static IEnumerable<IDialogPartResult> GetDialogPartResultsByPartIdentifier(this IDialogContext context,
                                                                                      string dialogPartIdentifier)
        => context.Results.Where(x => x.DialogPartId == dialogPartIdentifier);
}
