namespace DialogFramework.Domain.TestData;

[ExcludeFromCodeCoverage]
public static class DialogFixture
{
    public static IDialog Create(IDialogDefinitionIdentifier currentDialogDefinitionIdentifier)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogDefinitionIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder().WithValue("Empty"))
            .Build();

    public static IDialog Create(IDialogDefinitionIdentifier currentDialogDefinitionIdentifier,
                                 IDialogPartIdentifier currentPartIdentifier)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogDefinitionIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPartIdentifier))
            .WithCurrentState(DialogState.InProgress)
            .Build();

    public static IDialog Create(string id,
                                 IDialogDefinitionIdentifier currentDialogDefinitionIdentifier,
                                 IDialogPart currentPart)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(id))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogDefinitionIdentifier))
            .WithCurrentState(currentPart.GetState())
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .Build();

    public static IDialog Create(string id,
                                 IDialogDefinitionIdentifier currentDialogDefinitionIdentifier,
                                 IDialogPart currentPart,
                                 IEnumerable<IDialogPartResult> results)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(id))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogDefinitionIdentifier))
            .WithCurrentState(currentPart.GetState())
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .AddResults(results.Select(x => new DialogPartResultBuilder(x)))
            .Build();

    public static IDialog Create(IDialog source,
                                 IDialogDefinition definition,
                                 IReadOnlyCollection<IDialogPartResult> additionalAnswers)
        => new DialogBuilder(source, definition)
            .AddResults(additionalAnswers.Select(x => new DialogPartResultBuilder(x)))
            .Build();
}
