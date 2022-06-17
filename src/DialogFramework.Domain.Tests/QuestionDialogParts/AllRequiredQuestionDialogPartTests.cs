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
        var partResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id))
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
    public void Answers_From_Different_Question_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new AllRequiredQuestionDialogPartValidator())).Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut);
        var partResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder())
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();
        var results = new[] { partResult };

        // Act
        var result = sut.Validate(dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        result.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[]
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
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut);
        var partResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("A"))
            .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true))
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
            new DialogPartResultBuilder().WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id)).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("A")).WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true)).Build(),
            new DialogPartResultBuilder().WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id)).WithResultId(new DialogPartResultIdentifierBuilder().WithValue("B")).WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true)).Build()
        };

        // Act
        var result = sut.Validate(dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        result.Should().BeEmpty();
    }
}
