using DialogFramework.Domain.DialogPartResultValues.Builders;

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
            null
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
            null
        );

    public static IDialog Create(string id,
                                 IDialogDefinitionIdentifier currentDialogDefinitionIdentifier,
                                 IDialogPart currentPart,
                                 IEnumerable<IDialogPartResult> results,
                                 IEnumerable<IDialogValidationResult> validationErrors,
                                 IError? error)
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
            .WithErrorInformation(error == null ? null : new ErrorBuilder(error))
            .Build();

    public static IDialog Create(IDialog source, IReadOnlyCollection<IDialogPartResult> additionalAnswers)
        => new DialogBuilder(source)
            .AddResults(additionalAnswers.Select(x => new DialogPartResultBuilder(x)))
            .Build();

    public static IDialog CreateErrorDialog(IDialogDefinition dialogDefinition, string id)
        => Create
        (
            id,
            dialogDefinition.Metadata,
            dialogDefinition.Parts.First(),
            new[]
            {
                new DialogPartResultBuilder()
                    .WithDialogPartId(new DialogPartIdentifierBuilder(dialogDefinition.Parts.First().Id))
                    .WithResultId(new DialogPartResultIdentifierBuilder(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().Results.First().Id))
                    .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true))
                    .Build()
            },
            new[]
            {
                new DialogValidationResultBuilder()
                    .WithErrorMessage("You fool! You provided the wrong input")
                    .Build()
            },
            new ErrorBuilder().WithMessage("Kaboom").Build()
        );
}
