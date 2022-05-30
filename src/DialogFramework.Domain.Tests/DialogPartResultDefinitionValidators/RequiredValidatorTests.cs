namespace DialogFramework.Domain.Tests.DialogPartResultDefinitionValidators;

public class RequiredValidatorTests
{
    [Fact]
    public void Validate_Returns_Empty_Result_When_Value_Is_Provided()
    {
        // Arrange
        var sut = new RequiredValidator();
        var dialogMock = new Mock<IDialog>();
        dialogMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var context = DialogContextFixture.Create(dialogMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifier("PartId"));
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns(new DialogPartResultIdentifier("PartResultId"));

        // Act
        var actual = sut.Validate(context, dialogMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, new[] { new DialogPartResult(dialogPartMock.Object.Id, dialogPartResultDefinitionMock.Object.Id, new TextDialogPartResultValue("value")) });

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void Validate_Returns_Empty_Result_When_Value_Is_Provided_Multiple_Times_And_OccurenceCheck_Is_False()
    {
        // Arrange
        var sut = new RequiredValidator(false);
        var dialogMock = new Mock<IDialog>();
        dialogMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var context = DialogContextFixture.Create(dialogMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifier("PartId"));
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns(new DialogPartResultIdentifier("PartResultId"));

        // Act
        var actual = sut.Validate(context, dialogMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, new[] { new DialogPartResult(dialogPartMock.Object.Id, dialogPartResultDefinitionMock.Object.Id, new TextDialogPartResultValue("value")), new DialogPartResult(dialogPartMock.Object.Id, dialogPartResultDefinitionMock.Object.Id, new TextDialogPartResultValue("value")) });

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void Validate_Returns_NonEmpty_Result_When_Value_Is_Provided_Multiple_Times_And_OccurenceCheck_Is_True()
    {
        // Arrange
        var sut = new RequiredValidator(true);
        var dialogMock = new Mock<IDialog>();
        dialogMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var context = DialogContextFixture.Create(dialogMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifier("PartId"));
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns(new DialogPartResultIdentifier("PartResultId"));

        // Act
        var actual = sut.Validate(context, dialogMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, new[] { new DialogPartResult(dialogPartMock.Object.Id, dialogPartResultDefinitionMock.Object.Id, new TextDialogPartResultValue("value")), new DialogPartResult(dialogPartMock.Object.Id, dialogPartResultDefinitionMock.Object.Id, new TextDialogPartResultValue("value")) });

        // Assert
        actual.Should().ContainSingle();
        actual.First().ErrorMessage.Should().Be("Result value of [DialogPartIdentifier { Value = PartId }.DialogPartResultIdentifier { Value = PartResultId }] is only allowed one time");
    }

    [Fact]
    public void Validate_Returns_NonEmpty_Result_When_Value_Is_Not_Provided()
    {
        // Arrange
        var sut = new RequiredValidator();
        var dialogMock = new Mock<IDialog>();
        dialogMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var context = DialogContextFixture.Create(dialogMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifier("PartId"));
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns(new DialogPartResultIdentifier("PartResultId"));

        // Act
        var actual = sut.Validate(context, dialogMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, Enumerable.Empty<IDialogPartResult>());

        // Assert
        actual.Should().ContainSingle();
        actual.First().ErrorMessage.Should().Be("Result value of [DialogPartIdentifier { Value = PartId }.DialogPartResultIdentifier { Value = PartResultId }] is required");
    }
}
