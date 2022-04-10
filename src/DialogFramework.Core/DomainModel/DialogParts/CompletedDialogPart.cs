namespace DialogFramework.Core.DomainModel.DialogParts;

public record CompletedDialogPart : ICompletedDialogPart
{
    public CompletedDialogPart(string id,
                               string message,
                               IDialogPartGroup group)
    {
        Id = id;
        Message = message;
        Group = group;
    }
    public string Message { get; }
    public IDialogPartGroup Group { get; }
    public string Id { get; }
    public DialogState State => DialogState.Completed;
}
