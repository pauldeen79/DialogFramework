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
        => Create
        (
            id,
            currentDialogDefinitionIdentifier,
            currentPart,
            results,
            Enumerable.Empty<IDialogValidationResult>(),
            Enumerable.Empty<IError>()
        );

    public static IDialog Create(string id,
                             IDialogDefinitionIdentifier currentDialogDefinitionIdentifier,
                             IDialogPart currentPart,
                             IEnumerable<IDialogPartResult> results,
                             IEnumerable<IDialogValidationResult> validationErrors)
        => Create
        (
            id,
            currentDialogDefinitionIdentifier,
            currentPart,
            results,
            validationErrors,
            Enumerable.Empty<IError>()
        );

    public static IDialog Create(string id,
                                 IDialogDefinitionIdentifier currentDialogDefinitionIdentifier,
                                 IDialogPart currentPart,
                                 IEnumerable<IDialogPartResult> results,
                                 IEnumerable<IDialogValidationResult> validationErrors,
                                 IEnumerable<IError> errors)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(id))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(currentDialogDefinitionIdentifier))
            .WithCurrentPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .WithCurrentGroupId(currentPart.GetGroupId() == null
                ? null
                : new DialogPartGroupIdentifierBuilder(currentPart.GetGroupId()!))
            .WithCurrentState(currentPart.GetState())
            .AddResults(results.Select(x => new DialogPartResultBuilder(x)))
            .AddValidationErrors(validationErrors.Select(x => new DialogValidationResultBuilder(x)))
            .AddErrors(errors.Select(x => new ErrorBuilder(x)))
            .Build();

    public static IDialog Create(IDialog source, IReadOnlyCollection<IDialogPartResult> additionalAnswers)
        => new DialogBuilder(source)
            .AddResults(additionalAnswers.Select(x => new DialogPartResultBuilder(x)))
            .Build();
}
