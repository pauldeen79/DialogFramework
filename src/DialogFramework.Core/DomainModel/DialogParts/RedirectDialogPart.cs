namespace DialogFramework.Core.DomainModel.DialogParts;

public record RedirectDialogPart : IRedirectDialogPart
{
    public RedirectDialogPart(string id,
                              IDialog redirectDialog)
    {
        Id = id;
        RedirectDialog = redirectDialog;
    }

    public IDialog RedirectDialog { get; }
    public string Id { get; }
    public DialogState State => DialogState.InProgress;
}
