namespace DialogFramework.Core.DomainModel;

public class DialogPartGroup : IDialogPartGroup
{
    public DialogPartGroup(string id,
                           string title,
                           int groupNumber)
    {
        Id = id;
        Title = title;
        GroupNumber = groupNumber;
    }

    public string Id { get; }
    public string Title { get; }
    public int GroupNumber { get; }
}
