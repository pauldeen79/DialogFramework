namespace DialogFramework.Domain.Extensions;

public static class DialogExtensions
{
    public static IEnumerable<IDialogPartResult> GetDialogPartResultsByPartIdentifier(this IDialog instance,
                                                                                      IDialogPartIdentifier dialogPartIdentifier)
        => instance.Results.Where(x => Equals(x.DialogPartId, dialogPartIdentifier));
}
