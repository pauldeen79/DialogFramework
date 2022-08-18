namespace DialogFramework.Domain.Tests.DialogPartResultAnswerDefinitionValidators;

public class RequiredValidatorTests
{
    [Fact]
    public void Validate_Returns_Empty_Result_When_Value_Is_Provided()
    {
        // Arrange
        var sut = new RequiredValidator();
        var dialogDefinitionMock = new Mock<IDialogDefinition>();
        dialogDefinitionMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var dialog = DialogFixture.Create(dialogDefinitionMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifier("PartId"));
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultAnswerDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns(new DialogPartResultIdentifier("PartResultId"));

        // Act
        var actual = sut.Validate(dialog, dialogDefinitionMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, new[] { new DialogPartResultAnswer(dialogPartResultDefinitionMock.Object.Id, new DialogPartResultValueAnswer("value")) });

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void Validate_Returns_Empty_Result_When_Value_Is_Provided_Multiple_Times_And_OccurenceCheck_Is_False()
    {
        // Arrange
        var sut = new RequiredValidator(false);
        var dialogDefinitionMock = new Mock<IDialogDefinition>();
        dialogDefinitionMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var dialog = DialogFixture.Create(dialogDefinitionMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifier("PartId"));
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultAnswerDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns(new DialogPartResultIdentifier("PartResultId"));

        // Act
        var actual = sut.Validate(dialog, dialogDefinitionMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, new[]
        {
            new DialogPartResultAnswer(dialogPartResultDefinitionMock.Object.Id, new DialogPartResultValueAnswer("value")),
            new DialogPartResultAnswer(dialogPartResultDefinitionMock.Object.Id, new DialogPartResultValueAnswer("value"))
        });

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void Validate_Returns_NonEmpty_Result_When_Value_Is_Provided_Multiple_Times_And_OccurenceCheck_Is_True()
    {
        // Arrange
        var sut = new RequiredValidator(true);
        var dialogDefinitionMock = new Mock<IDialogDefinition>();
        dialogDefinitionMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var dialog = DialogFixture.Create(dialogDefinitionMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifier("PartId"));
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultAnswerDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns(new DialogPartResultIdentifier("PartResultId"));

        // Act
        var actual = sut.Validate(dialog, dialogDefinitionMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, new[]
        {
            new DialogPartResultAnswer(dialogPartResultDefinitionMock.Object.Id, new DialogPartResultValueAnswer("value")),
            new DialogPartResultAnswer(dialogPartResultDefinitionMock.Object.Id, new DialogPartResultValueAnswer("value"))
        });

        // Assert
        actual.Should().ContainSingle();
        actual.First().ErrorMessage.Should().Be("Result value of [DialogPartIdentifier { Value = PartId }.DialogPartResultIdentifier { Value = PartResultId }] is only allowed one time");
    }

    [Fact]
    public void Validate_Returns_NonEmpty_Result_When_Value_Is_Not_Provided()
    {
        // Arrange
        var sut = new RequiredValidator();
        var dialogDefinitionMock = new Mock<IDialogDefinition>();
        dialogDefinitionMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var dialog = DialogFixture.Create(dialogDefinitionMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifier("PartId"));
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultAnswerDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns(new DialogPartResultIdentifier("PartResultId"));

        // Act
        var actual = sut.Validate(dialog, dialogDefinitionMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, Enumerable.Empty<IDialogPartResultAnswer>());

        // Assert
        actual.Should().ContainSingle();
        actual.First().ErrorMessage.Should().Be("Result value of [DialogPartIdentifier { Value = PartId }.DialogPartResultIdentifier { Value = PartResultId }] is required");
    }

    [Fact]
    public void Validate_Returns_NonEmpty_Result_When_Value_Is_Provided_For_Other_Result()
    {
        // Arrange
        var sut = new RequiredValidator();
        var dialogDefinitionMock = new Mock<IDialogDefinition>();
        dialogDefinitionMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var dialog = DialogFixture.Create(dialogDefinitionMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifier("PartId"));
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultAnswerDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns(new DialogPartResultIdentifier("PartResultId"));
        var result = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var actual = sut.Validate(dialog, dialogDefinitionMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, new[] { result });

        // Assert
        actual.Should().ContainSingle();
        actual.First().ErrorMessage.Should().Be("Result value of [DialogPartIdentifier { Value = PartId }.DialogPartResultIdentifier { Value = PartResultId }] is required");
    }
}
