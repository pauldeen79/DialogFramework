namespace DialogFramework.Domain.DialogPartResultDefinitions;

public record EmptyDialogPartResultDefinition : IDialogPartResultDefinition
{
    public IDialogPartResultIdentifier Id => new DialogPartResultIdentifier(string.Empty);
    public string Title => string.Empty;
    public ResultValueType ValueType => ResultValueType.None;

    public IReadOnlyCollection<IDialogPartResultDefinitionValidator> Validators
        => new ReadOnlyValueCollection<IDialogPartResultDefinitionValidator>();

    public IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                         IDialogDefinition dialog,
                                                         IDialogPart dialogPart,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
        => Enumerable.Empty<IDialogValidationResult>();
}
