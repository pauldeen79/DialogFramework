namespace DialogFramework.Domain.Tests.Fixtures;

internal record DialogContextFixture : DialogContext
{
    public DialogContextFixture(IDialogIdentifier currentDialogIdentifier)
        : base(Guid.NewGuid().ToString(), currentDialogIdentifier, new EmptyDialogPart(), null, DialogState.Initial, new ValueCollection<IDialogPartResult>(), null)
    {
    }

    public DialogContextFixture(string id, IDialogIdentifier currentDialogIdentifier, IDialogPart currentPart, DialogState currentState)
        : base(id, currentDialogIdentifier, currentPart, null, currentState, new ValueCollection<IDialogPartResult>(), null)
    {
    }

    public void AddAnswer(IDialogPartResult result) => ((ValueCollection<IDialogPartResult>)Answers).Add(result);

    private sealed class EmptyDialogPart : IDialogPart
    {
        public string Id => "Empty";
    }
}
