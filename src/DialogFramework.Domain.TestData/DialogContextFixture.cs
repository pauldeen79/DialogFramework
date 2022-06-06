namespace DialogFramework.Domain.TestData;

public static class DialogContextFixture
{
    public static IDialogContext Create(IDialogIdentifier currentDialogIdentifier)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(currentDialogIdentifier))
            .WithCurrentState(DialogState.Initial)
            .WithCurrentPartId(new DialogPartIdentifierBuilder().WithValue("Empty"))
            .Build();

    public static IDialogContext Create(IDialogIdentifier currentDialogIdentifier, IDialogPartIdentifier currentPartIdentifier)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(currentDialogIdentifier))
            .WithCurrentState(DialogState.Initial)
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPartIdentifier))
            .Build();

    public static IDialogContext Create(string id, IDialogIdentifier currentDialogIdentifier, IDialogPart currentPart, DialogState currentState)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder().WithValue(id))
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(currentDialogIdentifier))
            .WithCurrentState(currentState)
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .Build();

    public static IDialogContext Create(string id, IDialogIdentifier currentDialogIdentifier, IDialogPart currentPart, IDialogPartGroup? currentGroup, DialogState currentState, IEnumerable<IDialogPartResult> results)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder().WithValue(id))
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(currentDialogIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .WithCurrentGroupId(currentGroup == null ? new DialogPartGroupIdentifierBuilder() : new DialogPartGroupIdentifierBuilder(currentGroup.Id))
            .WithCurrentState(currentState)
            .AddResults(results.Select(x => new DialogPartResultBuilder(x)))
            .Build();

    public static IDialogContext Create(IDialogContext source, IReadOnlyCollection<IDialogPartResult> additionalAnswers)
        => new DialogContextBuilder(source)
            .AddResults(additionalAnswers.Select(x => new DialogPartResultBuilder(x)))
            .Build();
}
