namespace DialogFramework.Core.DomainModel;

public record QuestionDialogPartResult : IDialogPartResultDefinition
{
    public QuestionDialogPartResult(string id,
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
