namespace DialogFramework.Application.Requests;

public record StartRequest : IRequest<Result<IDialog>>
{
    public IDialogDefinitionIdentifier DialogDefinitionIdentifier { get; }
    public IReadOnlyCollection<IDialogPartResult> DialogPartResults { get; }

    public StartRequest(IDialogDefinitionIdentifier dialogDefinitionIdentifier)
        : this(dialogDefinitionIdentifier, Enumerable.Empty<IDialogPartResult>())
    {
    }

    public StartRequest(IDialogDefinitionIdentifier dialogDefinitionIdentifier,
                        IEnumerable<IDialogPartResult> dialogPartResults)
    {
        DialogDefinitionIdentifier = dialogDefinitionIdentifier ?? throw new ArgumentNullException(nameof(dialogDefinitionIdentifier));
        DialogPartResults = new ReadOnlyValueCollection<IDialogPartResult>(dialogPartResults ?? throw new ArgumentNullException(nameof(dialogPartResults)));
    }
}
