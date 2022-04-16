namespace DialogFramework.Core.DomainModel.DialogPartResultDefinitions;

public record DialogPartResultDefinition : IDialogPartResultDefinition
{
    public DialogPartResultDefinition(string id,
                                      string title,
                                      ResultValueType valueType)
    {
        Id = id;
        Title = title;
        ValueType = valueType;
    }

    public string Id { get; }
    public string Title { get; }
    public ResultValueType ValueType { get; }
}
