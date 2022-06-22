namespace DialogFramework.Domain.Tests.QuestionDialogParts;

public class SingleOptionalQuestionDialogPartTests
{
    [Fact]
    public void No_Answers_Gives_No_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleOptionalQuestionDialogPartValidator())).Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut);
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .WithValue(new DialogPartResultValueAnswerBuilder())
            .Build();
        var results = new[] { partResult };

        // Act
        var result = sut.Validate(dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void One_Answer_Gives_No_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleOptionalQuestionDialogPartValidator())).Build();
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
        result.Should().BeEmpty();
    }

    [Fact]
    public void Two_Answers_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleOptionalQuestionDialogPartValidator())).Build();
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
        result.Should().ContainSingle();
        result.Single().ErrorMessage.Should().Be("Only one answer is allowed");
    }
}
