namespace DialogFramework.Core.DomainModel.DialogPartResultDefinitions;

public record DialogPartResultDefinition : IDialogPartResultDefinition
{
    public DialogPartResultDefinition(string id,
                                      string title,
                                      ResultValueType valueType)
        : this(id, title, valueType, Enumerable.Empty<IDialogPartResultDefinitionValidator>())
    {
    }

    public DialogPartResultDefinition(string id,
                                      string title,
                                      ResultValueType valueType,
                                      IEnumerable<IDialogPartResultDefinitionValidator> validators)
    {
        Id = id;
        Title = title;
        ValueType = valueType;
        Validators = new ValueCollection<IDialogPartResultDefinitionValidator>(validators);
    }

    public string Id { get; }
    public string Title { get; }
    public ResultValueType ValueType { get; }
    public ValueCollection<IDialogPartResultDefinitionValidator> Validators { get; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext, IDialogPartResult dialogPartResult)
    {
        foreach (var validator in Validators)
        {
            foreach (var validationError in validator.Validate(validationContext, dialogPartResult))
            {
                yield return validationError;
            }
        }
    }
}
