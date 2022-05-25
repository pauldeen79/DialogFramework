namespace DialogFramework.Core.Tests.QuestionDialogParts;

public class SingleRequiredQuestionDialogPartTests
{
    [Fact]
    public void No_Answers_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator())).Build();
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var dialogRepositoryMock = new Mock<IDialogRepository>();
        dialogRepositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, dialogRepositoryMock.Object, new Mock<IConditionEvaluator>().Object);

        // Act
        var actual = service.Continue(context, new[] { new DialogPartResultBuilder().WithDialogPartId(sut.Id).WithValue(new DialogPartResultValueBuilder()).Build() });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<IQuestionDialogPart>();
        var currentPart = (IQuestionDialogPart)actual.CurrentPart;
        currentPart.ValidationErrors.Should().ContainSingle();
        currentPart.ValidationErrors.Single().ErrorMessage.Should().Be("Answer is required");
    }

    [Fact]
    public void One_Answer_Gives_No_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator())).Build();
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var dialogRepositoryMock = new Mock<IDialogRepository>();
        dialogRepositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, dialogRepositoryMock.Object, new Mock<IConditionEvaluator>().Object);

        // Act
        var actual = service.Continue(context, new[] { new DialogPartResultBuilder().WithDialogPartId(sut.Id).WithResultId("A").WithValue(new DialogPartResultValueBuilder().WithResultValueType(ResultValueType.YesNo).WithValue(true)).Build() });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<IMessageDialogPart>();
        actual.CurrentState.Should().Be(DialogState.InProgress);
    }

    [Fact]
    public void Two_Answers_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator())).Build();
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var dialogRepositoryMock = new Mock<IDialogRepository>();
        dialogRepositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, dialogRepositoryMock.Object, new Mock<IConditionEvaluator>().Object);

        // Act
        var actual = service.Continue(context, new[]
        {
            new DialogPartResultBuilder().WithDialogPartId(sut.Id).WithResultId("A").WithValue(new DialogPartResultValueBuilder().WithResultValueType(ResultValueType.YesNo).WithValue(true)).Build(),
            new DialogPartResultBuilder().WithDialogPartId(sut.Id).WithResultId("B").WithValue(new DialogPartResultValueBuilder().WithResultValueType(ResultValueType.YesNo).WithValue(true)).Build()
        });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<IQuestionDialogPart>();
        var currentPart = (IQuestionDialogPart)actual.CurrentPart;
        currentPart.ValidationErrors.Should().ContainSingle();
        currentPart.ValidationErrors.Single().ErrorMessage.Should().Be("Only one answer is allowed");
    }
}
