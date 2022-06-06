namespace DialogFramework.Domain.Tests.QuestionDialogParts;

public class SingleRequiredQuestionDialogPartTests
{
    [Fact]
    public void No_Answers_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator())).Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut, DialogState.InProgress);
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .WithValue(new DialogPartResultValueBuilder())
            .Build();
        var results = new[] { result };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        actual.Should().ContainSingle();
        actual.Single().ErrorMessage.Should().Be("Answer is required");
    }

    [Fact]
    public void One_Answer_Gives_No_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator())).Build();
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
        actual.Should().BeEmpty();
    }

    [Fact]
    public void Two_Answers_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator())).Build();
        var dialDefinitionog = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialDefinitionog.Metadata, sut, DialogState.InProgress);
        var results = new[]
        {
            new DialogPartResultBuilder().WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id)).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("A")).WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true)).Build(),
            new DialogPartResultBuilder().WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id)).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("B")).WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true)).Build()
        };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, dialog, dialDefinitionog, results).ValidationErrors;

        // Assert
        actual.Should().ContainSingle();
        actual.Single().ErrorMessage.Should().Be("Only one answer is allowed");
    }
}
