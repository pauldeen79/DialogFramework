namespace DialogFramework.Core.Tests;

public class DialogServiceTests
{
    [Theory]
    [InlineData(DialogState.Aborted)]
    [InlineData(DialogState.Completed)]
    [InlineData(DialogState.ErrorOccured)]
    [InlineData(DialogState.InProgress)]
    public void Abort_Returns_ErrorDialogPart_When_Already_Aborted(DialogState currentState)
    {
        // Arrange
        var dialog = CreateDialog();
        var abortedPart = dialog.Parts.OfType<IAbortedDialogPart>().Single();
        var context = new DialogContextFixture(dialog, abortedPart, null, currentState, null);
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        errorDialogPart.ErrorMessage.Should().Be("Dialog has already been aborted");
    }

    [Fact]
    public void Abort_Returns_AbortDialogPart_Dialog_When_Possible()
    {
        // Arrange
        var dialog = CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var abortedPart = dialog.Parts.OfType<IAbortedDialogPart>().Single();
        var context = new DialogContextFixture(dialog, questionPart, questionPart.Group, DialogState.InProgress, null);
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);

        // Act
        var actual = sut.Abort(context);

        // Assert
        actual.State.Should().Be(DialogState.Aborted);
        actual.CurrentPart.Should().Be(abortedPart);
    }

    [Theory]
    [InlineData(DialogState.Aborted)]
    [InlineData(DialogState.Completed)]
    [InlineData(DialogState.ErrorOccured)]
    public void Continue_Returns_ErrorDialogPart_On_Invalid_State(DialogState currentState)
    {
        // Arrange
        var dialog = CreateDialog();
        IDialogPart currentPart = currentState switch
        {
            DialogState.Aborted => dialog.Parts.OfType<IAbortedDialogPart>().Single(),
            DialogState.Completed => dialog.Parts.OfType<ICompletedDialogPart>().First(),
            DialogState.ErrorOccured => dialog.Parts.OfType<IErrorDialogPart>().First(),
            _ => throw new NotImplementedException()
        };
        var context = new DialogContextFixture(dialog, currentPart, null, currentState, null);
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Continue(context, Enumerable.Empty<KeyValuePair<string, object?>>());

        // Assert
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        errorDialogPart.ErrorMessage.Should().Be($"Can only continue when the dialog is in progress. Current state is {currentState}");
    }

    [Fact]
    public void Continue_Returns_Next_DialogPart_When_Current_State_Is_Question_And_Answer_Is_Valid()
    {
        // Arrange
        var dialog = CreateDialog();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = new DialogContextFixture(dialog, currentPart, null, currentState, null);
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Continue(context, new[] { new KeyValuePair<string, object?>("Great", null) });

        // Assert
        result.CurrentPart.Should().BeAssignableTo<ICompletedDialogPart>();
    }

    [Fact]
    public void Continue_Returns_Same_DialogPart_When_Current_State_Is_Question_And_Answer_Is_Not_Valid()
    {
        // Arrange
        var dialog = CreateDialog();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = new DialogContextFixture(dialog, currentPart, null, currentState, null);
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Continue(context, new[] { new KeyValuePair<string, object?>("Unknown answer", null) });

        // Assert
        result.CurrentPart.Should().BeAssignableTo<QuestionDialogPartFixture>();
        var questionDialogPart = (QuestionDialogPartFixture)result.CurrentPart;
        questionDialogPart.ErrorMessage.Should().Be("Unknown answer: [Unknown answer]");
    }

    private static Dialog CreateDialog()
    {
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var welcomePart = new MessageDialogPart("Message1", "Welcome! I would like to answer a question", group1);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!");
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var answer1 = new QuestionDialogPartAnswer("Great", "I feel great, thank you!", AnswerValueType.None, _ => string.Empty, () => string.Empty);
        var answer2 = new QuestionDialogPartAnswer("Okay", "I feel kind of okay", AnswerValueType.None, _ => string.Empty, () => string.Empty);
        var answer3 = new QuestionDialogPartAnswer("Terrible", "I feel terrible, don't want to talk about it", AnswerValueType.None, _ => string.Empty, () => string.Empty);
        var questionPart = new QuestionDialogPartFixture("Question1", "How do you feel?", group1, new[] { answer1, answer2, answer3 }, values =>
        {
            if (!values.Any())
            {
                return "No answer selected";
            }

            if (values.Count() > 1)
            {
                return "Too many answers selected";
            }

            if (!new[] { "Great", "Okay", "Terrible" }.Contains(values.First().Key))
            {
                return $"Unknown answer: [{values.First().Key}]";
            }

            // If we've made it up to here, everything is okay! (exactly one valid answer)
            return null;
        });
        var completedPart = new CompletedDialogPart("Complete", "Thank you for your input!", group2);
        var parts = new IDialogPart[] { welcomePart, questionPart, completedPart, errorDialogPart, abortedPart };
        return new Dialog(
            "Test",
            "1.0.0",
            parts,
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
            );
    }
}
