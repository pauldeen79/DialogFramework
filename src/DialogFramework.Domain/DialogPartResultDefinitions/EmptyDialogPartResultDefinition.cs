namespace DialogFramework.Domain.DialogPartResultDefinitions;

public record EmptyDialogPartResultDefinition : IDialogPartResultDefinition
{
    public IDialogPartResultIdentifier Id => new DialogPartResultIdentifier(string.Empty);
    public string Title => string.Empty;
    public ResultValueType ValueType => ResultValueType.None;

    public IReadOnlyCollection<IDialogPartResultDefinitionValidator> Validators
        => new ReadOnlyValueCollection<IDialogPartResultDefinitionValidator>();

    public IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                         IDialogDefinition dialogDefinition,
                                                         IDialogPart dialogPart,
                                                         IEnumerable<IDialogPartResultAnswer> dialogPartResults)
        => Enumerable.Empty<IDialogValidationResult>();
}
