namespace DialogFramework.Domain.Tests.QuestionDialogParts;

public class AllRequiredQuestionDialogPartTests
{
    [Fact]
    public void No_Answers_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new AllRequiredQuestionDialogPartValidator())).Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut);
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();
        var results = new[] { partResult };

        // Act
        var result = sut.Validate(dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        result.Should().ContainSingle();
        result.Single().ErrorMessage.Should().Be("All 2 answers are required");
    }

    [Fact]
    public void One_Answer_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new AllRequiredQuestionDialogPartValidator())).Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut);
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("A"))
            .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(true))
            .Build();
        var results = new[] { partResult };

        // Act
        var result = sut.Validate(dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        result.Should().ContainSingle();
        result.Single().ErrorMessage.Should().Be("All 2 answers are required");
    }

    [Fact]
    public void Two_Answers_Gives_No_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new AllRequiredQuestionDialogPartValidator())).Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut);
        var results = new[]
        {
            new DialogPartResultAnswerBuilder().WithResultId(new DialogPartResultIdentifierBuilder().WithValue("A")).WithValue(new DialogPartResultValueAnswerBuilder().WithValue(true)).Build(),
            new DialogPartResultAnswerBuilder().WithResultId(new DialogPartResultIdentifierBuilder().WithValue("B")).WithValue(new DialogPartResultValueAnswerBuilder().WithValue(true)).Build()
        };

        // Act
        var result = sut.Validate(dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        result.Should().BeEmpty();
    }
}
