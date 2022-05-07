namespace DialogFramework.Core.DomainModel.DialogPartResultDefinitions;

public record EmptyDialogPartResultDefinition : IDialogPartResultDefinition
{
    public string Id => string.Empty;
    public string Title => string.Empty;
    public ResultValueType ValueType => ResultValueType.None;

    public ValueCollection<IDialogPartResultDefinitionValidator> Validators => new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext,
                                                  IDialogPart dialogPart,
                                                  IEnumerable<IDialogPartResult> dialogPartResults)
        => Enumerable.Empty<ValidationResult>();
}
