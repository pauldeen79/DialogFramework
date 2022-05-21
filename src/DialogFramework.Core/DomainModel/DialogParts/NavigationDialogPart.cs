namespace DialogFramework.Core.DomainModel.DialogParts;

public abstract record NavigationDialogPart : INavigationDialogPart
{
    protected NavigationDialogPart(string id)
        => Id = id;

    public abstract string GetNextPartId(IDialogContext context);
    public string Id { get; }
    public DialogState State => DialogState.InProgress;
}
