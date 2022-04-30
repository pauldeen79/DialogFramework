﻿namespace DialogFramework.Core.Tests;

public class DialogContextTests
{
    private static string Id => Guid.NewGuid().ToString();

    [Fact]
    public void GetProvidedAnswerByPart_Returns_Empty_Result_When_No_Provided_Answers_Found_In_Current_Context()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var welcomePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog, welcomePart, DialogState.InProgress);

        // Act
        var result = context.GetDialogPartResultsByPart(questionPart);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetProvidedAnswerByPart_Returns_Correct_Result_When_Provided_Answers_Found_In_Current_Context()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        IDialogContext context = new DialogContextFixture(Id, dialog, questionPart, DialogState.InProgress);
        context = context.AddDialogPartResults(new[] { new DialogPartResult(questionPart.Id, questionPart.Results.First().Id) });

        // Act
        var result = context.GetDialogPartResultsByPart(questionPart);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void ProvideAnswers_Overwrites_Answers_When_Already_Filled_In()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        IDialogContext context = new DialogContextFixture(Id, dialog, questionPart, DialogState.InProgress);

        // Act 1 - Call GetProvidedAnswerByPart first time, after initial provided answer
        context = context.AddDialogPartResults(new[] { new DialogPartResult(questionPart.Id, questionPart.Results.First().Id) });
        // Assert 1
        context.GetDialogPartResultsByPart(questionPart).Should().ContainSingle();
        context.GetDialogPartResultsByPart(questionPart).Single().ResultId.Should().Be(questionPart.Results.First().Id);

        // Act 2 - Call GetProvidedAnswerByPart second time, after changing the provided answer
        context = context.AddDialogPartResults(new[] { new DialogPartResult(questionPart.Id, questionPart.Results.Last().Id) });
        // Assert 2
        context.GetDialogPartResultsByPart(questionPart).Should().ContainSingle();
        context.GetDialogPartResultsByPart(questionPart).Single().ResultId.Should().Be(questionPart.Results.Last().Id);
    }

    [Fact]
    public void ResetProvidedAnswerByPart_Removes_ProvidedAnswer_Correctly()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        IDialogContext context = new DialogContextFixture(Id, dialog, questionPart, DialogState.InProgress);
        context = context.AddDialogPartResults(new[] { new DialogPartResult(questionPart.Id, questionPart.Results.First().Id) });
        context.GetDialogPartResultsByPart(questionPart).Should().ContainSingle();
        context.GetDialogPartResultsByPart(questionPart).Single().ResultId.Should().Be(questionPart.Results.First().Id);

        // Act 1 - Call reset while there is an answer
        context = context.ResetDialogPartResultByPart(questionPart);
        // Assert 1
        context.GetDialogPartResultsByPart(questionPart).Should().BeEmpty();

        // Act 2 - Call reset while there is no answer
        context = context.ResetDialogPartResultByPart(questionPart);
        // Assert 2
        context.GetDialogPartResultsByPart(questionPart).Should().BeEmpty();
    }
}
