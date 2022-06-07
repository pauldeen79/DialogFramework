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
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("C"))
            .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true))
            .Build();
        var results = new[] { result };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        actual.Should().ContainSingle();
        actual.Single().ErrorMessage.Should().Be("Unknown Result Id: [DialogPartResultIdentifier { Value = C }]");
    }

    [Fact]
    public void Validate_With_Known_Id_And_Wrong_ValueType_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().Build();
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut);
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(sut.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("A"))
            .Build();
        var results = new[] { result };

        // Act
        var actual = QuestionDialogPartFixture.Validate(sut, dialog, dialogDefinition, results).ValidationErrors;

        // Assert
        actual.Should().ContainSingle();
        actual.Single().ErrorMessage.Should().Be("Result for [DialogPartIdentifier { Value = Test }.DialogPartResultIdentifier { Value = A }] should be of type [YesNo], but type [None] was answered");
    }

    [Fact]
    public void Validate_With_Known_Id_And_Correct_ValueType_Gives_No_ValidationError()
    {
        // Arrange

        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = dialogDefinition.Parts.OfType<IQuestionDialogPart>().First();
        var dialog = DialogFixture.Create("Id", dialogDefinition.Metadata, sut);
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
    public void Can_Convert_Entity_To_Builder()
    {
        // Arrange
        var input = (IDialogPart)new QuestionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Test"))
            .WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder()))
            .Build();

        // Act
        var actual = input.CreateBuilder();

        // Assert
        actual.Build().Should().BeEquivalentTo(input);
    }

    [Fact]
    public void Can_Convert_Builder_To_Entity()
    {
        // Arrange
        var input = (IDialogPartBuilder)new QuestionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Test"))
            .WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder()));

        // Act
        var actual = input.Build();

        // Assert
        actual.Should().BeEquivalentTo(((QuestionDialogPartBuilder)input).Build());
    }

    [Fact]
    public void Constructing_QuestionDialogPart_With_Duplicate_Ids_Throws_ValidationException()
    {
        // Arrange
        var result = new DialogPartResultDefinitionBuilder()
            .WithId(new DialogPartResultIdentifierBuilder().WithValue("Test"));
        var builder = new QuestionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder())
            .WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder()))
            .AddResults(result, result);

        // Act
        var act = new Action(() => _ = builder.Build());

        // Assert
        act.Should().ThrowExactly<ValidationException>();
    }
}
