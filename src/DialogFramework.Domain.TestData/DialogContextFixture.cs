namespace DialogFramework.Domain.TestData;

public record DialogContextFixture : DialogContext
{
    public DialogContextFixture(IDialogIdentifier currentDialogIdentifier)
        : base(Guid.NewGuid().ToString(), currentDialogIdentifier, new EmptyDialogPart(), null, DialogState.Initial, new ReadOnlyValueCollection<IDialogPartResult>(), null)
    {
    }

    public DialogContextFixture(string id, IDialogIdentifier currentDialogIdentifier, IDialogPart currentPart, DialogState currentState)
        : base(id, currentDialogIdentifier, currentPart, null, currentState, new ReadOnlyValueCollection<IDialogPartResult>(), null)
    {
    }

    public DialogContextFixture(string id, IDialogIdentifier currentDialogIdentifier, IDialogPart currentPart, IDialogPartGroup? currentGroup, DialogState currentState, IReadOnlyCollection<IDialogPartResult> answers, Exception? exception)
        : base(id, currentDialogIdentifier, currentPart, currentGroup, currentState, answers, exception)
    {
    }

    public DialogContextFixture(DialogContext source, IReadOnlyCollection<IDialogPartResult> additionalAnswers)
        : base(source.Id, source.CurrentDialogIdentifier, source.CurrentPart, source.CurrentGroup, source.CurrentState, source.Answers.Concat(additionalAnswers).ToList(), source.Exception)
    {
    }

    private sealed class EmptyDialogPart : IDialogPart
    {
        public string Id => "Empty";
        public DialogState GetState() => DialogState.Initial;
    }
}
