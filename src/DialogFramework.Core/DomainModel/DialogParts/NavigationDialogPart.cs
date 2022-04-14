namespace DialogFramework.Core.DomainModel.DialogParts;

public abstract record NavigationDialogPart : INavigationDialogPart
{
    protected NavigationDialogPart(string id)
        => Id = id;

    public abstract IDialogPart GetNextPart(IDialogContext context);
    public string Id { get; }
    public DialogState State => DialogState.InProgress;
}
