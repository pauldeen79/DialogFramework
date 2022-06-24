namespace DialogFramework.Domain.TestData;

[ExcludeFromCodeCoverage]
public static class DialogFixture
{
    public static Dialog Create(IDialogDefinitionIdentifier currentDialogDefinitionIdentifier)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogDefinitionIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder().WithValue("Empty"))
            .Build();

    public static Dialog Create(IDialogDefinitionIdentifier currentDialogDefinitionIdentifier,
                                 IDialogPartIdentifier currentPartIdentifier)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogDefinitionIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPartIdentifier))
            .Build();

    public static Dialog Create(string id,
                                 IDialogDefinitionIdentifier currentDialogDefinitionIdentifier,
                                 IDialogPart currentPart)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(id))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogDefinitionIdentifier))
            .WithCurrentState(currentPart.GetState())
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .Build();

    public static Dialog Create(string id,
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

    public static Dialog Create(Dialog source, IReadOnlyCollection<IDialogPartResult> additionalAnswers)
        => new DialogBuilder(source)
            .AddResults(additionalAnswers.Select(x => new DialogPartResultBuilder(x)))
            .Build();
}
