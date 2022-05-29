namespace DialogFramework.Domain.TestData;

public record DialogContextFixture : DialogContext
{
    public DialogContextFixture(IDialogIdentifier currentDialogIdentifier)
        : base(Guid.NewGuid().ToString(), currentDialogIdentifier, "Empty", null, DialogState.Initial, Enumerable.Empty<IDialogPartResult>(), Enumerable.Empty<IDialogValidationResult>())
    {
    }

    public DialogContextFixture(string id, IDialogIdentifier currentDialogIdentifier, IDialogPart currentPart, DialogState currentState)
        : base(id, currentDialogIdentifier, currentPart.Id, null, currentState, Enumerable.Empty<IDialogPartResult>(), Enumerable.Empty<IDialogValidationResult>())
    {
    }

    public DialogContextFixture(string id, IDialogIdentifier currentDialogIdentifier, IDialogPart currentPart, IDialogPartGroup? currentGroup, DialogState currentState, IEnumerable<IDialogPartResult> results)
        : base(id, currentDialogIdentifier, currentPart.Id, currentGroup?.Id, currentState, results, Enumerable.Empty<IDialogValidationResult>())
    {
    }

    public DialogContextFixture(IDialogContext source, IReadOnlyCollection<IDialogPartResult> additionalAnswers)
        : base(source.Id, source.CurrentDialogIdentifier, source.CurrentPartId, source.CurrentGroupId, source.CurrentState, source.Results.Concat(additionalAnswers), Enumerable.Empty<IDialogValidationResult>())
    {
    }
}
