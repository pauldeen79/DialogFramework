namespace DialogFramework.Core.DomainModel.DialogPartResultDefinitions;

public record DefaultDialogPartResultDefinition : IDialogPartResultDefinition
{
    public DefaultDialogPartResultDefinition(string id,
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

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext, IDialogPartResult dialogPartResult)
        => Enumerable.Empty<ValidationResult>();
}
