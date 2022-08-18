namespace DialogFramework.Application.Requests;

public record StartRequest : IRequest<Result<IDialog>>
{
    public IDialogDefinitionIdentifier DialogDefinitionIdentifier { get; }
    public IReadOnlyCollection<IDialogPartResult> DialogPartResults { get; }
    public IReadOnlyCollection<IProperty> Properties { get; }

    public StartRequest(IDialogDefinitionIdentifier dialogDefinitionIdentifier)
        : this(dialogDefinitionIdentifier, Enumerable.Empty<IDialogPartResult>(), Enumerable.Empty<IProperty>())
    {
    }

    public StartRequest(IDialogDefinitionIdentifier dialogDefinitionIdentifier,
                        IEnumerable<IDialogPartResult> answers,
                        IEnumerable<IProperty> properties)
    {
        DialogDefinitionIdentifier = dialogDefinitionIdentifier ?? throw new ArgumentNullException(nameof(dialogDefinitionIdentifier));
        DialogPartResults = new ReadOnlyValueCollection<IDialogPartResult>(answers ?? throw new ArgumentNullException(nameof(answers)));
        Properties = new ReadOnlyValueCollection<IProperty>(properties ?? throw new ArgumentNullException(nameof(properties)));
    }
}
