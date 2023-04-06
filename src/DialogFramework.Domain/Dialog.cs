namespace DialogFramework.Domain;

public partial record Dialog
{
    public static Dialog Create(DialogDefinition definition, IEnumerable<DialogPartResult> results, object? context = null, string? id = null)
        => new(id ?? Guid.NewGuid().ToString(), definition.Id, definition.Version, results, context, true);
}
