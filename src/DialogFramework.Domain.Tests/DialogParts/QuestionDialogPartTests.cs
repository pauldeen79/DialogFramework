namespace DialogFramework.Domain.Tests.DialogParts;

public class QuestionDialogPartTests
{
    [Fact]
    public void Validate_With_Unknown_Id_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut);
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("C"))
            .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(true))
            .Build();
        var results = new[] { partResult };

        // Act
        var result = sut.Validate(dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        result.Should().ContainSingle();
        result.Single().ErrorMessage.Should().Be("Unknown Result Id: [DialogPartResultIdentifier { Value = C }]");
    }

    [Fact]
    public void Validate_With_Known_Id_And_Correct_ValueType_Gives_No_ValidationError()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = dialogDefinition.Parts.OfType<IQuestionDialogPart>().First();
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
    public void Can_Convert_Entity_To_Builder()
    {
        // Arrange
        var input = (IDialogPart)new QuestionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Test"))
            .WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder()))
            .AddAnswers(new DialogPartResultAnswerDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder()))
            .Build();

        // Act
        var result = input.CreateBuilder();

        // Assert
        result.Build().Should().BeEquivalentTo(input);
    }

    [Fact]
    public void Can_Convert_Builder_To_Entity()
    {
        // Arrange
        var input = (IDialogPartBuilder)new QuestionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Test"))
            .WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder()))
            .AddAnswers(new DialogPartResultAnswerDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder()));

        // Act
        var result = input.Build();

        // Assert
        result.Should().BeEquivalentTo(((QuestionDialogPartBuilder)input).Build());
    }

    [Fact]
    public void Constructing_QuestionDialogPart_With_Duplicate_Ids_Throws_ValidationException()
    {
        // Arrange
        var result = new DialogPartResultAnswerDefinitionBuilder()
            .WithId(new DialogPartResultIdentifierBuilder().WithValue("Test"));
        var builder = new QuestionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder())
            .WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder()))
            .AddAnswers(result, result);

        // Act
        var act = new Action(() => _ = builder.Build());

        // Assert
        act.Should().ThrowExactly<ValidationException>();
    }

    [Fact]
    public void Constructing_QuestionDialogPart_Without_Results_Throws_ValidationException()
    {
        // Arrange
        var builder = new QuestionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder())
            .WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder()));

        // Act
        var act = new Action(() => _ = builder.Build());

        // Assert
        act.Should().ThrowExactly<ValidationException>();
    }
}
