namespace DialogFramework.Domain.Extensions;

public static class DialogDefinitionExtensions
{
    public static IEnumerable<IDialogPart> GetAllParts(this IDialogDefinition instance)
        => new IDialogPart[] { instance.AbortedPart, instance.CompletedPart, instance.ErrorPart }.Concat(instance.Parts);
}
