namespace DialogFramework.Domain.TestData;

public static class DialogContextFixture
{
    public static IDialogContext Create(IDialogIdentifier currentDialogIdentifier)
        => new DialogContextBuilder()
            .WithId(Guid.NewGuid().ToString())
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(currentDialogIdentifier))
            .WithCurrentState(DialogState.Initial)
            .WithCurrentPartId("Empty")
            .Build();

    public static IDialogContext Create(string id, IDialogIdentifier currentDialogIdentifier, IDialogPart currentPart, DialogState currentState)
        => new DialogContextBuilder()
            .WithId(id)
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(currentDialogIdentifier))
            .WithCurrentState(currentState)
            .WithCurrentPartId(currentPart.Id)
            .Build();

    public static IDialogContext Create(string id, IDialogIdentifier currentDialogIdentifier, IDialogPart currentPart, IDialogPartGroup? currentGroup, DialogState currentState, IEnumerable<IDialogPartResult> results)
        => new DialogContextBuilder()
            .WithId(id)
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(currentDialogIdentifier))
            .WithCurrentPartId(currentPart.Id)
            .WithCurrentGroupId(currentGroup?.Id)
            .WithCurrentState(currentState)
            .AddResults(results.Select(x => new DialogPartResultBuilder(x)))
            .Build();

    public static IDialogContext Create(IDialogContext source, IReadOnlyCollection<IDialogPartResult> additionalAnswers)
        => new DialogContextBuilder(source)
            .AddResults(additionalAnswers.Select(x => new DialogPartResultBuilder(x)))
            .Build();
}
