namespace DialogFramework.Domain.Tests.QuestionDialogParts;

public class AllRequiredQuestionDialogPartTests
{
    [Fact]
    public void No_Answers_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new AllRequiredQuestionDialogPartValidator())).Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut, DialogState.InProgress);
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();
        var results = new[] { result };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        actual.Should().ContainSingle();
        actual.Single().ErrorMessage.Should().Be("All 2 answers are required");
    }

    [Fact]
    public void Answers_From_Different_Question_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new AllRequiredQuestionDialogPartValidator())).Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut, DialogState.InProgress);
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder())
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();
        var results = new[] { result };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        actual.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
        {
            "Provided answer from wrong question",
            "All 2 answers are required"
        });
    }
    [Fact]
    public void One_Answer_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new AllRequiredQuestionDialogPartValidator())).Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut, DialogState.InProgress);
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("A"))
            .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true))
            .Build();
        var results = new[] { result };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        actual.Should().ContainSingle();
        actual.Single().ErrorMessage.Should().Be("All 2 answers are required");
    }

    [Fact]
    public void Two_Answers_Gives_No_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new AllRequiredQuestionDialogPartValidator())).Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut, DialogState.InProgress);
        var results = new[]
        {
            new DialogPartResultBuilder().WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id)).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("A")).WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true)).Build(),
            new DialogPartResultBuilder().WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id)).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("B")).WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true)).Build()
        };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        actual.Should().BeEmpty();
    }
}
