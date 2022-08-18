namespace DialogFramework.Domain.Tests.Extensions;

public class DialogExtensionsTests
{
    private static string Id => Guid.NewGuid().ToString();

    [Fact]
    public void GetDialogPartResultsByPartIdentifier_Returns_Empty_Result_When_No_Provided_Answers_Found_In_Current_Context()
    {
        // Arrange
        var definition = DialogDefinitionFixture.CreateBuilder().Build();
        var welcomePart = definition.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = definition.Parts.OfType<IQuestionDialogPart>().Single();
        var dialog = DialogFixture.Create(Id, definition.Metadata, welcomePart);

        // Act
        var result = dialog.GetDialogPartResultsByPartIdentifier(questionPart.Id);

        // Assert
        result.GetValueOrThrow().Should().BeEmpty();
    }

    [Fact]
    public void GetDialogPartResultsByPartIdentifier_Returns_Correct_Result_When_Provided_Answers_Found_In_Current_Context()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        dialog.Continue(dialogDefinition, new[] { new DialogPartResultAnswer(questionPart.Answers.First().Id, new DialogPartResultValueAnswer(null)) }, conditionEvaluatorMock.Object);

        // Act
        var result = dialog.GetDialogPartResultsByPartIdentifier(questionPart.Id);

        // Assert
        result.Should().NotBeNull();
    }
}
