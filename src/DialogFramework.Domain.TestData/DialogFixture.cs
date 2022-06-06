namespace DialogFramework.Domain.TestData;

public static class DialogFixture
{
    public static IDialog Create(IDialogDefinitionIdentifier currentDialogIdentifier)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogIdentifier))
            .WithCurrentState(DialogState.Initial)
            .WithCurrentPartId(new DialogPartIdentifierBuilder().WithValue("Empty"))
            .Build();

    public static IDialog Create(IDialogDefinitionIdentifier currentDialogIdentifier, IDialogPartIdentifier currentPartIdentifier)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogIdentifier))
            .WithCurrentState(DialogState.Initial)
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPartIdentifier))
            .Build();

    public static IDialog Create(string id, IDialogDefinitionIdentifier currentDialogIdentifier, IDialogPart currentPart, DialogState currentState)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder().WithValue(id))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogIdentifier))
            .WithCurrentState(currentState)
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .Build();

    public static IDialog Create(string id, IDialogDefinitionIdentifier currentDialogIdentifier, IDialogPart currentPart, IDialogPartGroup? currentGroup, DialogState currentState, IEnumerable<IDialogPartResult> results)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder().WithValue(id))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .WithCurrentGroupId(currentGroup == null ? new DialogPartGroupIdentifierBuilder() : new DialogPartGroupIdentifierBuilder(currentGroup.Id))
            .WithCurrentState(currentState)
            .AddResults(results.Select(x => new DialogPartResultBuilder(x)))
            .Build();

    public static IDialog Create(IDialog source, IReadOnlyCollection<IDialogPartResult> additionalAnswers)
        => new DialogContextBuilder(source)
            .AddResults(additionalAnswers.Select(x => new DialogPartResultBuilder(x)))
            .Build();
}
