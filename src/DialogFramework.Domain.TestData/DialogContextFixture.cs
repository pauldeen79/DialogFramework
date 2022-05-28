namespace DialogFramework.Domain.TestData;

public record DialogContextFixture : DialogContext
{
    public DialogContextFixture(IDialogIdentifier currentDialogIdentifier)
        : base(Guid.NewGuid().ToString(), currentDialogIdentifier, new EmptyDialogPart(), null, DialogState.Initial, Enumerable.Empty<IDialogPartResult>())
    {
    }

    public DialogContextFixture(string id, IDialogIdentifier currentDialogIdentifier, IDialogPart currentPart, DialogState currentState)
        : base(id, currentDialogIdentifier, currentPart, null, currentState, Enumerable.Empty<IDialogPartResult>())
    {
    }

    public DialogContextFixture(string id, IDialogIdentifier currentDialogIdentifier, IDialogPart currentPart, IDialogPartGroup? currentGroup, DialogState currentState, IEnumerable<IDialogPartResult> results)
        : base(id, currentDialogIdentifier, currentPart, currentGroup, currentState, results)
    {
    }

    public DialogContextFixture(IDialogContext source, IReadOnlyCollection<IDialogPartResult> additionalAnswers)
        : base(source.Id, source.CurrentDialogIdentifier, source.CurrentPart, source.CurrentGroup, source.CurrentState, source.Results.Concat(additionalAnswers))
    {
    }

    private sealed class EmptyDialogPart : IDialogPart
    {
        public string Id => "Empty";
        public DialogState GetState() => DialogState.Initial;
    }
}
