namespace DialogFramework.Domain.Extensions;

public static class DialogExtensions
{
    public static IEnumerable<IDialogPartResult> GetDialogPartResultsByPartIdentifier(this IDialog instance,
                                                                                      IDialogPartIdentifier dialogPartIdentifier)
        => instance.Results.Where(x => Equals(x.DialogPartId, dialogPartIdentifier));

    public static Result Error(this IDialog instance,
                               IDialogDefinition dialogDefinition,
                               params IError[] errorMessages)
        => instance.Error(dialogDefinition, errorMessages.AsEnumerable());
}
