namespace DialogFramework.Core.DomainModel.DialogParts;

public record RedirectDialogPart : IRedirectDialogPart
{
    public RedirectDialogPart(string id, IDialogMetadata redirectDialogMetadata)
    {
        Id = id;
        RedirectDialogMetadata = redirectDialogMetadata;
    }

    public IDialogMetadata RedirectDialogMetadata { get; }
    public string Id { get; }
    public DialogState State => DialogState.InProgress;
}
