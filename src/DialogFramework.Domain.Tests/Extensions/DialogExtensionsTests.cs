namespace DialogFramework.Domain.Tests.Extensions;

public class DialogExtensionsTests
{
    private static string Id => Guid.NewGuid().ToString();

    [Fact]
    public void GetDialogPartResultsByPartIdentifier_Returns_Empty_Result_When_No_Provided_Answers_Found_In_Current_Context()
    {
        // Arrange
        var dialog = DialogDefinitionFixture.CreateBuilder().Build();
        var welcomePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = DialogFixture.Create(Id, dialog.Metadata, welcomePart, DialogState.InProgress);

        // Act
        var result = context.GetDialogPartResultsByPartIdentifier(questionPart.Id);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetDialogPartResultsByPartIdentifier_Returns_Correct_Result_When_Provided_Answers_Found_In_Current_Context()
    {
        // Arrange
        var dialog = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        IDialog context = DialogFixture.Create(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        context = context.Chain(x => x.Continue(dialog, new[] { new DialogPartResult(questionPart.Id, questionPart.Results.First().Id, new EmptyDialogPartResultValue()) }, conditionEvaluatorMock.Object));

        // Act
        var result = context.GetDialogPartResultsByPartIdentifier(questionPart.Id);

        // Assert
        result.Should().NotBeNull();
    }
}
