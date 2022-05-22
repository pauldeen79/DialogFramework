﻿namespace DialogFramework.Core.Tests.QuestionDialogParts;

public class SingleOptionalQuestionDialogPartTests
{
    [Fact]
    public void No_Answers_Gives_No_ValidationError()
    {
        // Arrange
        var group = new DialogPartGroup("Group", "Group", 1);
        var sut = new SingleOptionalQuestionDialogPart("Test", "Max 1 answer", "Title", group, new[] { new DialogPartResultDefinition("A", "First", ResultValueType.YesNo), new DialogPartResultDefinition("B", "Second", ResultValueType.YesNo) });
        var dialog = new Dialog(new DialogMetadata("Test", "Test dialog", "1.0.0", true), new[] { sut }, new ErrorDialogPart("Error", "Something went wrong", null), new Mock<IAbortedDialogPart>().Object, new CompletedDialogPart("Completed", "Completed", "Thank you", group), new[] { group });
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, new TestDialogRepository());

        // Act
        var actual = service.Continue(context, new[] { new DialogPartResult(sut.Id) });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<ICompletedDialogPart>();
        actual.CurrentState.Should().Be(DialogState.Completed);
    }

    [Fact]
    public void One_Answer_Gives_No_ValidationError()
    {
        // Arrange
        var group = new DialogPartGroup("Group", "Group", 1);
        var sut = new SingleOptionalQuestionDialogPart("Test", "Max 1 answer", "Title", group, new[] { new DialogPartResultDefinition("A", "First", ResultValueType.YesNo), new DialogPartResultDefinition("B", "Second", ResultValueType.YesNo) });
        var dialog = new Dialog(new DialogMetadata("Test", "Test dialog", "1.0.0", true), new[] { sut }, new ErrorDialogPart("Error", "Something went wrong", null), new Mock<IAbortedDialogPart>().Object, new CompletedDialogPart("Completed", "Completed", "Thank you", group), new[] { group });
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, new TestDialogRepository());

        // Act
        var actual = service.Continue(context, new[] { new DialogPartResult(sut.Id, "A", new YesNoDialogPartResultValue(true)) });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<ICompletedDialogPart>();
        actual.CurrentState.Should().Be(DialogState.Completed);
    }

    [Fact]
    public void Two_Answers_Gives_ValidationError()
    {
        // Arrange
        var group = new DialogPartGroup("Group", "Group", 1);
        var sut = new SingleOptionalQuestionDialogPart("Test", "Max 1 answer", "Title", group, new[] { new DialogPartResultDefinition("A", "First", ResultValueType.YesNo), new DialogPartResultDefinition("B", "Second", ResultValueType.YesNo) });
        var dialog = new Dialog(new DialogMetadata("Test", "Test dialog", "1.0.0", true), new[] { sut }, new ErrorDialogPart("Error", "Something went wrong", null), new Mock<IAbortedDialogPart>().Object, new CompletedDialogPart("Completed", "Completed", "Thank you", group), new[] { group });
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, new TestDialogRepository());

        // Act
        var actual = service.Continue(context, new[]
        {
            new DialogPartResult(sut.Id, "A", new YesNoDialogPartResultValue(true)),
            new DialogPartResult(sut.Id, "B", new YesNoDialogPartResultValue(true))
        });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<SingleOptionalQuestionDialogPart>();
        var currentPart = (SingleOptionalQuestionDialogPart)actual.CurrentPart;
        currentPart.ValidationErrors.Should().ContainSingle();
        currentPart.ValidationErrors.Single().ErrorMessage.Should().Be("Only one answer is allowed");
    }
}
