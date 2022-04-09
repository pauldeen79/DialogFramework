namespace DialogFramework.Core.DomainModel.DialogParts;

public record AbortedDialogPart : IAbortedDialogPart
{
    public AbortedDialogPart(string id, string message)
    {
        Id = id;
        Message = message;
    }

    public string Message { get; }
    public string Id { get; }
}
