namespace DialogFramework.Domain.Tests.DialogPartResultDefinitionValidators;

public class OccurenceCountValidatorTests
{
    [Fact]
    public void Validate_Returns_Empty_Result_When_OccurenceCount_Is_Correct()
    {
        // Arrange
        var sut = new OccurenceCountValidator(1);
        var dialogDefinitionMock = new Mock<IDialogDefinition>();
        dialogDefinitionMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var dialog = DialogFixture.Create(dialogDefinitionMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifier("PartId"));
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns(new DialogPartResultIdentifier("PartResultId"));

        // Act
        var actual = sut.Validate(dialog, dialogDefinitionMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, new[] { new DialogPartResultAnswer(new EmptyDialogPartResultDefinition().Id, new DialogPartResultValueBuilder().Build()) });

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void Validate_Returns_NonEmpty_Result_When_OccurenceCount_Is_Not_Correct_And_Times_Is_One()
    {
        // Arrange
        var sut = new OccurenceCountValidator(1);
        var dialogDefinitionMock = new Mock<IDialogDefinition>();
        dialogDefinitionMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var definition = DialogFixture.Create(dialogDefinitionMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifier("PartId"));
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns(new DialogPartResultIdentifier("PartResultId"));

        // Act
        var actual = sut.Validate(definition, dialogDefinitionMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, Enumerable.Empty<IDialogPartResultAnswer>());

        // Assert
        actual.Should().ContainSingle();
        actual.First().ErrorMessage.Should().Be("Result value of [DialogPartIdentifier { Value = PartId }.DialogPartResultIdentifier { Value = PartResultId }] is required");
    }

    [Fact]
    public void Validate_Returns_NonEmpty_Result_When_OccurenceCount_Is_Not_Correct_And_Times_Is_Larger_Than_One_And_Range()
    {
        // Arrange
        var sut = new OccurenceCountValidator(1, 2);
        var dialogDefinitionMock = new Mock<IDialogDefinition>();
        dialogDefinitionMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var dialog = DialogFixture.Create(dialogDefinitionMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifier("PartId"));
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns(new DialogPartResultIdentifier("PartResultId"));

        // Act
        var actual = sut.Validate(dialog, dialogDefinitionMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, Enumerable.Empty<IDialogPartResultAnswer>());

        // Assert
        actual.Should().ContainSingle();
        actual.First().ErrorMessage.Should().Be("Result value of [DialogPartIdentifier { Value = PartId }.DialogPartResultIdentifier { Value = PartResultId }] should be supplied between 1 and 2 times");
    }

    [Fact]
    public void Validate_Returns_NonEmpty_Result_When_OccurenceCount_Is_Not_Correct_And_Times_Is_Larger_Than_One_And_No_Range()
    {
        // Arrange
        var sut = new OccurenceCountValidator(2, 2);
        var dialogDefinitionMock = new Mock<IDialogDefinition>();
        dialogDefinitionMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var dialog = DialogFixture.Create(dialogDefinitionMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifier("PartId"));
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns(new DialogPartResultIdentifier("PartResultId"));

        // Act
        var actual = sut.Validate(dialog, dialogDefinitionMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, Enumerable.Empty<IDialogPartResultAnswer>());

        // Assert
        actual.Should().ContainSingle();
        actual.First().ErrorMessage.Should().Be("Result value of [DialogPartIdentifier { Value = PartId }.DialogPartResultIdentifier { Value = PartResultId }] should be supplied 2 times");
    }
}
