﻿namespace DialogFramework.Domain.Tests.QuestionDialogParts;

public class AllRequiredQuestionDialogPartTests
{
    [Fact]
    public void No_Answers_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new AllRequiredQuestionDialogPartValidator())).Build();
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var dialogRepositoryMock = new Mock<IDialogRepository>();
        dialogRepositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, dialogRepositoryMock.Object, new Mock<IConditionEvaluator>().Object);
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(sut.Id)
            .Build();

        // Act
        var actual = service.Continue(context, new[] { result });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<IQuestionDialogPart>();
        var currentPart = (IQuestionDialogPart)actual.CurrentPart;
        currentPart.ValidationErrors.Should().ContainSingle();
        currentPart.ValidationErrors.Single().ErrorMessage.Should().Be("All 2 answers are required");
    }

    [Fact]
    public void One_Answer_Gives_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new AllRequiredQuestionDialogPartValidator())).Build();
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var dialogRepositoryMock = new Mock<IDialogRepository>();
        dialogRepositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, dialogRepositoryMock.Object, new Mock<IConditionEvaluator>().Object);
        var result = new DialogPartResultBuilder()
            .WithDialogPartId(sut.Id)
            .WithResultId("A")
            .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true))
            .Build();

        // Act
        var actual = service.Continue(context, new[] { result });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<IQuestionDialogPart>();
        var currentPart = (IQuestionDialogPart)actual.CurrentPart;
        currentPart.ValidationErrors.Should().ContainSingle();
        currentPart.ValidationErrors.Single().ErrorMessage.Should().Be("All 2 answers are required");
    }

    [Fact]
    public void Two_Answers_Gives_No_ValidationError()
    {
        // Arrange
        var sut = QuestionDialogPartFixture.CreateBuilder().AddValidators(new QuestionDialogPartValidatorBuilder(new AllRequiredQuestionDialogPartValidator())).Build();
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture("Id", dialog.Metadata, sut, DialogState.InProgress);
        var dialogRepositoryMock = new Mock<IDialogRepository>();
        dialogRepositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var service = new DialogService(new Mock<IDialogContextFactory>().Object, dialogRepositoryMock.Object, new Mock<IConditionEvaluator>().Object);

        // Act
        var actual = service.Continue(context, new[]
        {
            new DialogPartResultBuilder().WithDialogPartId(sut.Id).WithResultId("A").WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true)).Build(),
            new DialogPartResultBuilder().WithDialogPartId(sut.Id).WithResultId("B").WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true)).Build()
        });

        // Assert
        actual.CurrentPart.Should().BeAssignableTo<IMessageDialogPart>();
        actual.CurrentState.Should().Be(DialogState.InProgress);
    }
}