namespace DialogFramework.Core.Tests;

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
}
