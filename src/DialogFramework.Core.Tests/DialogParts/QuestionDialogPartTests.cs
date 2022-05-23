namespace DialogFramework.Core.Tests.DialogParts;

public class QuestionDialogPartTests
{
    [Fact]
    public void Validate_With_Unknown_Id_Gives_ValidationError()
    {
        // Arrange
        var group = new DialogPartGroup("Group", "Group", 1);
        var sut = new QuestionDialogPart("Test", "Give me an answer!", "Title", group, new[] { new DialogPartResultDefinition("A", "First", ResultValueType.YesNo), new DialogPartResultDefinition("B", "Second", ResultValueType.YesNo) }, Enumerable.Empty<IQuestionDialogPartValidator>());
        var dialog = new Dialog(new DialogMetadata("Test", "Test dialog", "1.0.0", true), new[] { sut }, new ErrorDialogPart("Error", "Something went wrong", null), new Mock<IAbortedDialogPart>().Object, new CompletedDialogPart("Completed", "Completed", "Thank you", group), new[] { group });
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, new TestDialogRepository());

        // Act
        var actual = service.Continue(context, new[] { new DialogPartResult(sut.Id, "C", new YesNoDialogPartResultValue(true)) });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<QuestionDialogPart>();
        var currentPart = (QuestionDialogPart)actual.CurrentPart;
        currentPart.ValidationErrors.Should().ContainSingle();
        currentPart.ValidationErrors.Single().ErrorMessage.Should().Be("Unknown Result Id: [C]");
    }

    [Fact]
    public void Validate_With_Known_Id_And_Wrong_ValueType_Gives_ValidationError()
    {
        // Arrange
        var group = new DialogPartGroup("Group", "Group", 1);
        var sut = new QuestionDialogPart("Test", "Give me an answer!", "Title", group, new[] { new DialogPartResultDefinition("A", "First", ResultValueType.YesNo), new DialogPartResultDefinition("B", "Second", ResultValueType.YesNo) }, Enumerable.Empty<IQuestionDialogPartValidator>());
        var dialog = new Dialog(new DialogMetadata("Test", "Test dialog", "1.0.0", true), new[] { sut }, new ErrorDialogPart("Error", "Something went wrong", null), new Mock<IAbortedDialogPart>().Object, new CompletedDialogPart("Completed", "Completed", "Thank you", group), new[] { group });
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, new TestDialogRepository());

        // Act
        var actual = service.Continue(context, new[] { new DialogPartResult(sut.Id, "A", new EmptyDialogPartResultValue()) });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<QuestionDialogPart>();
        var currentPart = (QuestionDialogPart)actual.CurrentPart;
        currentPart.ValidationErrors.Should().ContainSingle();
        currentPart.ValidationErrors.Single().ErrorMessage.Should().Be("Result for [Test.A] should be of type [YesNo], but type [None] was answered");
    }

    [Fact]
    public void Validate_With_Known_Id_And_Correct_ValueType_Gives_No_ValidationError()
    {
        // Arrange
        var group = new DialogPartGroup("Group", "Group", 1);
        var sut = new QuestionDialogPart("Test", "Give me an answer!", "Title", group, new[] { new DialogPartResultDefinition("A", "First", ResultValueType.YesNo), new DialogPartResultDefinition("B", "Second", ResultValueType.YesNo) }, Enumerable.Empty<IQuestionDialogPartValidator>());
        var dialog = new Dialog(new DialogMetadata("Test", "Test dialog", "1.0.0", true), new[] { sut }, new ErrorDialogPart("Error", "Something went wrong", null), new Mock<IAbortedDialogPart>().Object, new CompletedDialogPart("Completed", "Completed", "Thank you", group), new[] { group });
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, new TestDialogRepository());

        // Act
        var actual = service.Continue(context, new[] { new DialogPartResult(sut.Id, "A", new YesNoDialogPartResultValue(true)) });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<ICompletedDialogPart>();
        actual.CurrentState.Should().Be(DialogState.Completed);
    }
}
