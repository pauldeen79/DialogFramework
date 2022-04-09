﻿namespace DialogFramework.Core.Tests;

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
        actual.CurrentState.Should().Be(DialogState.Aborted);
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
        result.CurrentPart.Id.Should().Be("Completed");
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

    [Fact]
    public void Continue_Uses_Result_From_DecisionPart_When_DecisionPart_Returns_No_Error()
    {
        // Arrange
        var dialog = CreateDialog();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var factory = new DialogContextFactoryFixture(_ => new DialogContextFixture(dialog, currentPart, null, currentState, null));
        var sut = new DialogService(factory);
        var context = sut.Start(dialog); // start the dialog, this will get the welcome messae
        context = sut.Continue(context, Enumerable.Empty<KeyValuePair<string, object?>>()); // skip the welcome message

        // Act
        var result = sut.Continue(context, new[] { new KeyValuePair<string, object?>("Terrible", null) }); // answer the question with 'Terrible', this will trigger a second message

        // Assert
        result.CurrentPart.Should().BeAssignableTo<IMessageDialogPart>();
        result.CurrentPart.Id.Should().Be("Message");
    }

    [Fact]
    public void Start_Throws_When_Context_Could_Not_Be_Created()
    {
        // Arrange
        var dialog = CreateDialog();
        var factory = new DialogContextFactoryFixture(_ => throw new InvalidOperationException("Kaboom"));
        var sut = new DialogService(factory);
        var start = new Action(() => sut.Start(dialog));

        // Act
        start.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    [Fact]
    public void Start_Returns_ErrorDialogPart_When_First_DialogPart_Could_Not_Be_Determined()
    {
        // Arrange
        var dialog = CreateDialog(false);
        var context = new DialogContextFixture(dialog, new ErrorDialogPart("Error", "Not initialized yet"), null, default, null);
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Start(dialog);

        // Assert
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        errorDialogPart.ErrorMessage.Should().Be("Could not determine next part. Dialog does not have any parts.");
    }

    [Fact]
    public void Start_Returns_First_DialogPart_When_It_Could_Be_Determined()
    {
        // Arrange
        var dialog = CreateDialog();
        var context = new DialogContextFixture(dialog, new ErrorDialogPart("Error", "Not initialized yet"), null, default, null);
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Start(dialog);

        // Assert
        result.CurrentPart.Should().BeAssignableTo<IMessageDialogPart>();
        result.CurrentPart.Id.Should().Be("Welcome");
    }

    private static Dialog CreateDialog(bool addParts = true)
    {
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var welcomePart = new MessageDialogPart("Welcome", "Welcome! I would like to answer a question", group1);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!");
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var answerGreat = new QuestionDialogPartAnswerFixture("Great", "I feel great, thank you!", AnswerValueType.None, _ => string.Empty, () => string.Empty);
        var answerOkay = new QuestionDialogPartAnswerFixture("Okay", "I feel kind of okay", AnswerValueType.None, _ => string.Empty, () => string.Empty);
        var answerTerrible = new QuestionDialogPartAnswerFixture("Terrible", "I feel terrible, don't want to talk about it", AnswerValueType.None, _ => string.Empty, () => string.Empty);
        var questionPart = new QuestionDialogPartFixture("Question1", "How do you feel?", group1, new[] { answerGreat, answerOkay, answerTerrible }, values =>
        {
            if (!values.Any())
            {
                return "No answer selected";
            }

            if (values.Count() > 1)
            {
                return "Too many answers selected";
            }

            if (!new[] { answerGreat.Id, answerOkay.Id, answerTerrible.Id }.Contains(values.First().Key))
            {
                return $"Unknown answer: [{values.First().Key}]";
            }

            // If we've made it up to here, everything is okay! (exactly one valid answer)
            return null;
        });
        var messagePart = new MessageDialogPart("Message", "I'm sorry to hear that. Let us know if we can do something to help you.", group1);
        var completedPart = new CompletedDialogPart("Completed", "Thank you for your input!", group2);
        var decisionPart = new DecisionDialogPartFixture
        (
            "Decision",
            (_, answers) => answers.Any(a => a.Key == answerTerrible.Id)
                ? messagePart
                : completedPart
        );
        var parts = new IDialogPart[] { welcomePart, questionPart, decisionPart, messagePart, completedPart, errorDialogPart, abortedPart }.Where(_ => addParts);
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
