﻿namespace DialogFramework.Core.Tests;

public class DialogContextTests
{
    [Fact]
    public void GetProvidedAnswerByPart_Returns_Null_When_No_Provided_Answers_Found_In_Current_Context()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var welcomePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(dialog, welcomePart, DialogState.InProgress);

        // Act
        var result = context.GetProvidedAnswerByPart(questionPart);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetProvidedAnswerByPart_Returns_Correct_Result_When_Provided_Answers_Found_In_Current_Context()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        IDialogContext context = new DialogContextFixture(dialog, questionPart, DialogState.InProgress);
        context = context.ProvideAnswers(new[] { new ProvidedAnswer(questionPart, questionPart.Answers.First(), new EmptyAnswerValue()) });

        // Act
        var result = context.GetProvidedAnswerByPart(questionPart);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void ProvideAnswers_Overwrites_Answers_When_Already_Filled_In()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        IDialogContext context = new DialogContextFixture(dialog, questionPart, DialogState.InProgress);

        // Act 1
        context = context.ProvideAnswers(new[] { new ProvidedAnswer(questionPart, questionPart.Answers.First(), new EmptyAnswerValue()) });
        // Assert 1
        context.GetProvidedAnswerByPart(questionPart).Should().NotBeNull();
        context.GetProvidedAnswerByPart(questionPart)!.Answer.Id.Should().Be(questionPart.Answers.First().Id);

        // Act 1
        context = context.ProvideAnswers(new[] { new ProvidedAnswer(questionPart, questionPart.Answers.Last(), new EmptyAnswerValue()) });
        // Assert 1
        context.GetProvidedAnswerByPart(questionPart).Should().NotBeNull();
        context.GetProvidedAnswerByPart(questionPart)!.Answer.Id.Should().Be(questionPart.Answers.Last().Id);
    }
}
