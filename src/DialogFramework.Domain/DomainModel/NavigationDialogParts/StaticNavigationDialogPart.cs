namespace DialogFramework.Domain.DomainModel.NavigationDialogParts;

public class StaticNavigationDialogPart : INavigationDialogPart
{
    public string Id { get; }
    public DialogState State { get; }

    private readonly string _nextPartId;

    public StaticNavigationDialogPart(string id, string nextPartId)
    {
        Id = id;
        State = DialogState.InProgress;
        _nextPartId = nextPartId;
    }

    public string GetNextPartId(IDialogContext context)
        => _nextPartId;
}
