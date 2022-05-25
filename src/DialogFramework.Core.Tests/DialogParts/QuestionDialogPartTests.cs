namespace DialogFramework.Core.Tests.DialogParts;

public class QuestionDialogPartTests
{
    [Fact]
    public void Validate_With_Unknown_Id_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().Build();
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, new TestDialogRepository(), new Mock<IConditionEvaluator>().Object);

        // Act
        var actual = service.Continue(context, new[] { new DialogPartResult(sut.Id, "C", new YesNoDialogPartResultValue(true)) });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<IQuestionDialogPart>();
        var currentPart = (IQuestionDialogPart)actual.CurrentPart;
        currentPart.ValidationErrors.Should().ContainSingle();
        currentPart.ValidationErrors.Single().ErrorMessage.Should().Be("Unknown Result Id: [C]");
    }

    [Fact]
    public void Validate_With_Known_Id_And_Wrong_ValueType_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().Build();
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, new TestDialogRepository(), new Mock<IConditionEvaluator>().Object);

        // Act
        var actual = service.Continue(context, new[] { new DialogPartResult(sut.Id, "A", new EmptyDialogPartResultValue()) });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<IQuestionDialogPart>();
        var currentPart = (IQuestionDialogPart)actual.CurrentPart;
        currentPart.ValidationErrors.Should().ContainSingle();
        currentPart.ValidationErrors.Single().ErrorMessage.Should().Be("Result for [Test.A] should be of type [YesNo], but type [None] was answered");
    }

    [Fact]
    public void Validate_With_Known_Id_And_Correct_ValueType_Gives_No_ValidationError()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var sut = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, new TestDialogRepository(), new Mock<IConditionEvaluator>().Object);

        // Act
        var actual = service.Continue(context, new[] { new DialogPartResult(sut.Id, "A", new YesNoDialogPartResultValue(true)) });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<IMessageDialogPart>();
        actual.CurrentState.Should().Be(DialogState.InProgress);
    }
}
