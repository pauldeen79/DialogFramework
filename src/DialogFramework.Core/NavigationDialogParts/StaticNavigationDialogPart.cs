namespace DialogFramework.Core.NavigationDialogParts;

public class StaticNavigationDialogPart : INavigationDialogPart
{
    public string Id { get; }
    public DialogState State { get; }

    private readonly IDialogPart _nextPart;

    public StaticNavigationDialogPart(string id, IDialogPart nextPart)
    {
        Id = id;
        State = DialogState.InProgress;
        _nextPart = nextPart;
    }

    public IDialogPart GetNextPart(IDialogContext context)
        => _nextPart;
}
