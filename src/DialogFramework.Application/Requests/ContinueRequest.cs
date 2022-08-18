namespace DialogFramework.Application.Requests;

public record ContinueRequest : IRequest<Result<IDialog>>
{
    public IDialog Dialog { get; }
    public IReadOnlyCollection<IDialogPartResultAnswer> DialogPartResults { get; }

    public ContinueRequest(IDialog dialog)
        : this(dialog, new[] { new DialogPartResultAnswer(new EmptyDialogPartResultAnswerDefinition().Id, new DialogPartResultValueAnswer(null)) })
    {
    }

    public ContinueRequest(IDialog dialog,
                           IEnumerable<IDialogPartResultAnswer> answers)
    {
        Dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
        DialogPartResults = new ReadOnlyValueCollection<IDialogPartResultAnswer>(answers ?? throw new ArgumentNullException(nameof(answers)));
    }
}
