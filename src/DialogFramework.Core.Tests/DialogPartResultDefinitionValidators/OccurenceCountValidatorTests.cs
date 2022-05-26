namespace DialogFramework.Core.Tests.DialogPartResultDefinitionValidators;

public class OccurenceCountValidatorTests
{
    [Fact]
    public void Validate_Returns_Empty_Result_When_OccurenceCount_Is_Correct()
    {
        // Arrange
        var sut = new OccurenceCountValidator(1);
        var dialogMock = new Mock<IDialog>();
        dialogMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var context = new DialogContextFixture(dialogMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns("PartId");
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns("PartResultId");

        // Act
        var actual = sut.Validate(context, dialogMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, new[] { new DialogPartResult(dialogPartMock.Object.Id, new EmptyDialogPartResultDefinition().Id, new EmptyDialogPartResultValue()) });

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void Validate_Returns_NonEmpty_Result_When_OccurenceCount_Is_Not_Correct_And_Times_Is_One()
    {
        // Arrange
        var sut = new OccurenceCountValidator(1);
        var dialogMock = new Mock<IDialog>();
        dialogMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var context = new DialogContextFixture(dialogMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns("PartId");
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns("PartResultId");

        // Act
        var actual = sut.Validate(context, dialogMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, Enumerable.Empty<IDialogPartResult>());

        // Assert
        actual.Should().ContainSingle();
        actual.First().ErrorMessage.Should().Be("Result value of [PartId.PartResultId] is required");
    }

    [Fact]
    public void Validate_Returns_NonEmpty_Result_When_OccurenceCount_Is_Not_Correct_And_Times_Is_Larger_Than_One_And_Range()
    {
        // Arrange
        var sut = new OccurenceCountValidator(1, 2);
        var dialogMock = new Mock<IDialog>();
        dialogMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var context = new DialogContextFixture(dialogMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns("PartId");
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns("PartResultId");

        // Act
        var actual = sut.Validate(context, dialogMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, Enumerable.Empty<IDialogPartResult>());

        // Assert
        actual.Should().ContainSingle();
        actual.First().ErrorMessage.Should().Be("Result value of [PartId.PartResultId] should be supplied between 1 and 2 times");
    }

    [Fact]
    public void Validate_Returns_NonEmpty_Result_When_OccurenceCount_Is_Not_Correct_And_Times_Is_Larger_Than_One_And_No_Range()
    {
        // Arrange
        var sut = new OccurenceCountValidator(2, 2);
        var dialogMock = new Mock<IDialog>();
        dialogMock.SetupGet(x => x.Metadata).Returns(new DialogMetadata("Test", true, "Test", "1.0.0.0"));
        var context = new DialogContextFixture(dialogMock.Object.Metadata);
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns("PartId");
        var dialogPartResultDefinitionMock = new Mock<IDialogPartResultDefinition>();
        dialogPartResultDefinitionMock.SetupGet(x => x.Id).Returns("PartResultId");

        // Act
        var actual = sut.Validate(context, dialogMock.Object, dialogPartMock.Object, dialogPartResultDefinitionMock.Object, Enumerable.Empty<IDialogPartResult>());

        // Assert
        actual.Should().ContainSingle();
        actual.First().ErrorMessage.Should().Be("Result value of [PartId.PartResultId] should be supplied 2 times");
    }
}
