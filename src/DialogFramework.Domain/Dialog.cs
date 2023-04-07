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
    {
        var dialogPartResult = Results.FirstOrDefault(x => x.PartId == partId);

        return dialogPartResult == null
            ? Result<object?>.NotFound($"Could not find dialog part result with id [{partId}]")
            : dialogPartResult.GetValue();
    }
}
