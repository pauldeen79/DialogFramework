using CrossCutting.Common.Extensions;

namespace DialogFramework.Domain.Tests;

public class DialogContextTests
{
    private static string Id => Guid.NewGuid().ToString();

    [Fact]
    public void GetDialogPartResultsByPartIdentifier_Returns_Empty_Result_When_No_Provided_Answers_Found_In_Current_Context()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var welcomePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = DialogContextFixture.Create(Id, dialog.Metadata, welcomePart, DialogState.InProgress);

        // Act
        var result = context.GetDialogPartResultsByPartIdentifier(questionPart.Id);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetDialogPartResultsByPartIdentifier_Returns_Correct_Result_When_Provided_Answers_Found_In_Current_Context()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        IDialogContext context = DialogContextFixture.Create(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        context = context.Chain(x => x.AddDialogPartResults(dialog, new[] { new DialogPartResult(questionPart.Id, questionPart.Results.First().Id, new EmptyDialogPartResultValue()) }));

        // Act
        var result = context.GetDialogPartResultsByPartIdentifier(questionPart.Id);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void AddDialogPartResults_Overwrites_Answers_When_Already_Filled_In()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        IDialogContext context = DialogContextFixture.Create(Id, dialog.Metadata, questionPart, DialogState.InProgress);

        // Act 1 - Call GetProvidedAnswerByPart first time, after initial provided answer
        context = context.Chain(x => x.AddDialogPartResults(dialog, new[] { new DialogPartResult(questionPart.Id, questionPart.Results.First().Id, new EmptyDialogPartResultValue()) }));
        // Assert 1
        context.GetDialogPartResultsByPartIdentifier(questionPart.Id).Should().ContainSingle();
        context.GetDialogPartResultsByPartIdentifier(questionPart.Id).Single().ResultId.Should().Be(questionPart.Results.First().Id);

        // Act 2 - Call GetProvidedAnswerByPart second time, after changing the provided answer
        context = context.Chain(x => x.AddDialogPartResults(dialog, new[] { new DialogPartResult(questionPart.Id, questionPart.Results.Last().Id, new EmptyDialogPartResultValue()) }));
        // Assert 2
        context.GetDialogPartResultsByPartIdentifier(questionPart.Id).Should().ContainSingle();
        context.GetDialogPartResultsByPartIdentifier(questionPart.Id).Single().ResultId.Should().Be(questionPart.Results.Last().Id);
    }

    [Fact]
    public void ResetCurrentState_Removes_ProvidedAnswer_Correctly()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        IDialogContext context = DialogContextFixture.Create(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        context = context.Chain(x => x.AddDialogPartResults(dialog, new[] { new DialogPartResult(questionPart.Id, questionPart.Results.First().Id, new EmptyDialogPartResultValue()) }));
        context.GetDialogPartResultsByPartIdentifier(questionPart.Id).Should().ContainSingle();
        context.GetDialogPartResultsByPartIdentifier(questionPart.Id).Single().ResultId.Should().Be(questionPart.Results.First().Id);

        // Act 1 - Call reset while there is an answer
        context = context.Chain(x => x.ResetCurrentState(dialog));
        // Assert 1
        context.GetDialogPartResultsByPartIdentifier(questionPart.Id).Should().BeEmpty();

        // Act 2 - Call reset while there is no answer
        context = context.Chain(x => x.ResetCurrentState(dialog));
        // Assert 2
        context.GetDialogPartResultsByPartIdentifier(questionPart.Id).Should().BeEmpty();
    }
}
