namespace DialogFramework.Application.Requests;

public record ContinueRequest : IRequest<Result<IDialog>>
{
    public IDialog Dialog { get; }
    public IReadOnlyCollection<IDialogPartResultAnswer> DialogPartResults { get; }

    public ContinueRequest(IDialog dialog)
        : this(dialog, new[] { new DialogPartResultAnswer(new EmptyDialogPartResultDefinition().Id, new DialogPartResultValueAnswer(null)) })
    {
    }

    public ContinueRequest(IDialog dialog,
                           IEnumerable<IDialogPartResultAnswer> dialogPartResults)
    {
        Dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
        DialogPartResults = new ReadOnlyValueCollection<IDialogPartResultAnswer>(dialogPartResults ?? throw new ArgumentNullException(nameof(dialogPartResults)));
    }
}
