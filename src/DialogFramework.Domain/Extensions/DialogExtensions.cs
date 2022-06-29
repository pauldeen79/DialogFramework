namespace DialogFramework.Domain.Extensions;

public static class DialogExtensions
{
    public static IEnumerable<IDialogPartResult> GetAllResults(this IDialog instance, IDialogDefinition definition)
        => definition.Parts.SelectMany(x => instance.GetDialogPartResultsByPartIdentifier(x.Id).GetValueOrThrow());
}
