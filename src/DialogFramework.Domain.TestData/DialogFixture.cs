namespace DialogFramework.Domain.TestData;

public static class DialogFixture
{
    public static IDialog Create(IDialogDefinitionIdentifier currentDialogDefinitionIdentifier)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogDefinitionIdentifier))
            .WithCurrentState(DialogState.Initial)
            .WithCurrentPartId(new DialogPartIdentifierBuilder().WithValue("Empty"))
            .Build();

    public static IDialog Create(IDialogDefinitionIdentifier currentDialogDefinitionIdentifier,
                                 IDialogPartIdentifier currentPartIdentifier)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogDefinitionIdentifier))
            .WithCurrentState(DialogState.Initial)
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPartIdentifier))
            .Build();

    public static IDialog Create(string id,
                                 IDialogDefinitionIdentifier currentDialogDefinitionIdentifier,
                                 IDialogPart currentPart,
                                 DialogState currentState)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(id))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogDefinitionIdentifier))
            .WithCurrentState(currentState)
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .Build();

    public static IDialog Create(string id,
                                 IDialogDefinitionIdentifier currentDialogDefinitionIdentifier,
                                 IDialogPart currentPart,
                                 IDialogPartGroup? currentGroup,
                                 DialogState currentState,
                                 IEnumerable<IDialogPartResult> results)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(id))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogDefinitionIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .WithCurrentGroupId(currentGroup == null ? new DialogPartGroupIdentifierBuilder() : new DialogPartGroupIdentifierBuilder(currentGroup.Id))
            .WithCurrentState(currentState)
            .AddResults(results.Select(x => new DialogPartResultBuilder(x)))
            .Build();

    public static IDialog Create(IDialog source, IReadOnlyCollection<IDialogPartResult> additionalAnswers)
        => new DialogBuilder(source)
            .AddResults(additionalAnswers.Select(x => new DialogPartResultBuilder(x)))
            .Build();
}
