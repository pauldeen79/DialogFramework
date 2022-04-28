﻿namespace DialogFramework.Core.Tests.DialogParts;

public class QuestionDialogPartTests
{
    [Fact]
    public void Validate_With_Unknown_Id_Gives_ValidationError()
    {
        // Arrange
        var group = new DialogPartGroup("Group", "Group", 1);
        var sut = new QuestionDialogPart("Test", "Give me an answer!", "Title", group, new[] { new DialogPartResultDefinition("A", "First", ResultValueType.YesNo), new DialogPartResultDefinition("B", "Second", ResultValueType.YesNo) });
        var dialog = new Dialog(new DialogMetadata("Test", "Test dialog", "1.0.0", true), new[] { sut }, new ErrorDialogPart("Error", "Something went wrong", null), new Mock<IAbortedDialogPart>().Object, new CompletedDialogPart("Completed", "Completed", "Thank you", group), new[] { group });
        var context = new DialogContextFixture("Id", dialog, sut, DialogState.InProgress);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object);

        // Act
        var actual = service.Continue(context, new[] { new DialogPartResult(sut, new DialogPartResultDefinition("C", "Title", ResultValueType.YesNo), new YesNoDialogPartResultValue(true)) });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<QuestionDialogPart>();
        var currentPart = (QuestionDialogPart)actual.CurrentPart;
        currentPart.ErrorMessages.Should().ContainSingle();
        currentPart.ErrorMessages.Single().Should().Be("Unknown result: [C]");
    }

    [Fact]
    public void Validate_With_Known_Id_And_Wrong_ValueType_Gives_ValidationError()
    {
        // Arrange
        var group = new DialogPartGroup("Group", "Group", 1);
        var sut = new QuestionDialogPart("Test", "Give me an answer!", "Title", group, new[] { new DialogPartResultDefinition("A", "First", ResultValueType.YesNo), new DialogPartResultDefinition("B", "Second", ResultValueType.YesNo) });
        var dialog = new Dialog(new DialogMetadata("Test", "Test dialog", "1.0.0", true), new[] { sut }, new ErrorDialogPart("Error", "Something went wrong", null), new Mock<IAbortedDialogPart>().Object, new CompletedDialogPart("Completed", "Completed", "Thank you", group), new[] { group });
        var context = new DialogContextFixture("Id", dialog, sut, DialogState.InProgress);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object);

        // Act
        var actual = service.Continue(context, new[] { new DialogPartResult(sut, new DialogPartResultDefinition("A", "Title", ResultValueType.YesNo), new EmptyDialogPartResultValue()) });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<QuestionDialogPart>();
        var currentPart = (QuestionDialogPart)actual.CurrentPart;
        currentPart.ErrorMessages.Should().ContainSingle();
        currentPart.ErrorMessages.Single().Should().Be("Result should be of type [YesNo], but type [None] was answered");
    }

    [Fact]
    public void Validate_With_Known_Id_And_Correct_ValueType_Gives_No_ValidationError()
    {
        // Arrange
        var group = new DialogPartGroup("Group", "Group", 1);
        var sut = new QuestionDialogPart("Test", "Give me an answer!", "Title", group, new[] { new DialogPartResultDefinition("A", "First", ResultValueType.YesNo), new DialogPartResultDefinition("B", "Second", ResultValueType.YesNo) });
        var dialog = new Dialog(new DialogMetadata("Test", "Test dialog", "1.0.0", true), new[] { sut }, new ErrorDialogPart("Error", "Something went wrong", null), new Mock<IAbortedDialogPart>().Object, new CompletedDialogPart("Completed", "Completed", "Thank you", group), new[] { group });
        var context = new DialogContextFixture("Id", dialog, sut, DialogState.InProgress);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object);

        // Act
        var actual = service.Continue(context, new[] { new DialogPartResult(sut, new DialogPartResultDefinition("A", "Title", ResultValueType.YesNo), new YesNoDialogPartResultValue(true)) });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<ICompletedDialogPart>();
        actual.CurrentState.Should().Be(DialogState.Completed);
    }
}