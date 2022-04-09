namespace DialogFramework.Core.DomainModel.DialogParts;

public record OutcomeDialogPart : IOutcomeDialogPart
{
    public OutcomeDialogPart(string id, string title)
    {
        Id = id;
        Title = title;
    }

    public string Title { get; }
    public string Id { get; }
}
