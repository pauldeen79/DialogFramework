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

    public Result<object?> GetResultValueByPartId(string partId)
        => Results.FirstOrDefault(x => x.PartId == partId) switch
        {
            DialogPartResult result => result.GetValue(),
            _ => Result<object?>.NotFound($"Could not find dialog part result with id [{partId}]")
        };
}
