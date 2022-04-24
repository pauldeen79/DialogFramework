namespace DialogFramework.Core.DomainModel;

public record DialogPartGroup : IDialogPartGroup
{
    public DialogPartGroup(string id,
                           string title,
                           int number)
    {
        Id = id;
        Title = title;
        Number = number;
    }

    public string Id { get; }
    public string Title { get; }
    public int Number { get; }
}
