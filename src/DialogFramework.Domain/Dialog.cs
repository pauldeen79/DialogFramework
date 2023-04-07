namespace DialogFramework.Domain;

public partial record Dialog
{
    public Dialog(
        DialogDefinition definition,
        IEnumerable<DialogPartResult>? results = null,
        object? context = null,
        string? id = null)
        : this(
              id ?? Guid.NewGuid().ToString(),
              definition.Id,
              definition.Version,
              results ?? Enumerable.Empty<DialogPartResult>(),
              context)
    {
    }
}
