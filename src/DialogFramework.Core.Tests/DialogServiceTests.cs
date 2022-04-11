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
        var context = new DialogContextFixture(dialog, abortedPart, null, currentState, null, Enumerable.Empty<IProvidedAnswer>());
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        errorDialogPart.Exception.Should().NotBeNull();
        errorDialogPart.Exception!.Message.Should().Be("Dialog has already been aborted");
    }

    [Fact]
    public void Abort_Returns_AbortDialogPart_Dialog_When_Possible()
    {
        // Arrange
        var dialog = CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var abortedPart = dialog.Parts.OfType<IAbortedDialogPart>().Single();
        var context = new DialogContextFixture(dialog, questionPart, questionPart.Group, DialogState.InProgress, null, Enumerable.Empty<IProvidedAnswer>());
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
        var context = new DialogContextFixture(dialog, currentPart, null, currentState, null, Enumerable.Empty<IProvidedAnswer>());
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Continue(context, Enumerable.Empty<IProvidedAnswer>());

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        errorDialogPart.Exception.Should().NotBeNull();
        errorDialogPart.Exception!.Message.Should().Be($"Can only continue when the dialog is in progress. Current state is {currentState}");
    }

    [Fact]
    public void Continue_Returns_Next_DialogPart_When_Current_State_Is_Question_And_Answer_Is_Valid()
    {
        // Arrange
        var dialog = CreateDialog();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = new DialogContextFixture(dialog, currentPart, null, currentState, null, Enumerable.Empty<IProvidedAnswer>());
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Continue(context, new[] { new ProvidedAnswer(currentPart, currentPart.Answers.Single(x => x.Id == "Great"), null) });

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
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
        var context = new DialogContextFixture(dialog, currentPart, null, currentState, null, Enumerable.Empty<IProvidedAnswer>());
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);
        var answerMock = new Mock<IQuestionDialogPartAnswer>();
        answerMock.SetupGet(x => x.Id).Returns("Unknown answer");

        // Act
        var result = sut.Continue(context, new[] { new ProvidedAnswer(currentPart, answerMock.Object, null) });

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentPart.Should().BeAssignableTo<QuestionDialogPartFixture>();
        var questionDialogPart = (QuestionDialogPartFixture)result.CurrentPart;
        questionDialogPart.ErrorMessage.Should().Be("Unknown answer: [Unknown answer]");
    }

    [Fact]
    public void Continue_Returns_Same_DialogPart_On_Answers_From_Wrong_Question()
    {
        // Arrange
        var dialog = CreateDialog();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = new DialogContextFixture(dialog, currentPart, null, currentState, null, Enumerable.Empty<IProvidedAnswer>());
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);
        var answerMock = new Mock<IQuestionDialogPartAnswer>();
        answerMock.SetupGet(x => x.Id).Returns("Unknown answer");

        // Act
        var result = sut.Continue(context, new[] { new ProvidedAnswer(new Mock<IQuestionDialogPart>().Object, answerMock.Object, null) });

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentPart.Should().BeAssignableTo<QuestionDialogPartFixture>();
        var questionDialogPart = (QuestionDialogPartFixture)result.CurrentPart;
        questionDialogPart.ErrorMessage.Should().Be("Provided answers from wrong question");
    }

    [Fact]
    public void Continue_Uses_Result_From_DecisionPart_When_DecisionPart_Returns_No_Error()
    {
        // Arrange
        var dialog = CreateDialog();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var factory = new DialogContextFactoryFixture(_ => new DialogContextFixture(dialog, currentPart, null, currentState, null, Enumerable.Empty<IProvidedAnswer>()));
        var sut = new DialogService(factory);
        var context = sut.Start(dialog); // start the dialog, this will get the welcome messae
        context = sut.Continue(context, Enumerable.Empty<IProvidedAnswer>()); // skip the welcome message

        // Act
        var result = sut.Continue(context, new[] { new ProvidedAnswer(currentPart, currentPart.Answers.Single(a => a.Id == "Terrible"), null) }); // answer the question with 'Terrible', this will trigger a second message

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentPart.Should().BeAssignableTo<IMessageDialogPart>();
        result.CurrentPart.Id.Should().Be("Message");
    }

    [Fact]
    public void Continue_Uses_Result_From_RedirectPart()
    {
        // Arrange
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var completedPart = new CompletedDialogPart("Completed", "Thank you for your input!", group2);
        var welcomePart = new MessageDialogPart("Welcome", "Welcome! I would like to answer a question", group1);
        var dialog2 = new Dialog
        (
            "Dialog2",
            "1.0.0",
            new IDialogPart[] { welcomePart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var redirectPart = new RedirectDialogPart("Redirect", dialog2);
        var dialog1 = new Dialog
        (
            "Dialog1",
            "1.0.0",
            new IDialogPart[] { welcomePart, redirectPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            Enumerable.Empty<IDialogPartGroup>()
        );
        var factory = new DialogContextFactoryFixture(d =>
            d.Id == dialog1.Id
                ? new DialogContextFixture(dialog1)
                : new DialogContextFixture(dialog2));
        var sut = new DialogService(factory);
        var context = sut.Start(dialog1); // this will trigger the message on dialog 1

        // Act
        var result = sut.Continue(context, Enumerable.Empty<IProvidedAnswer>()); // this will trigger the redirect to dialog 2

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentDialog.Id.Should().Be(dialog2.Id);
        result.CurrentPart.Id.Should().Be(welcomePart.Id);
    }

    [Theory]
    [InlineData(DialogState.Aborted)]
    [InlineData(DialogState.Completed)]
    [InlineData(DialogState.ErrorOccured)]
    public void Continue_Returns_ErrorDialogPart_When_State_Is_Wrong(DialogState currentState)
    {
        // Arrange
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var completedPart = new CompletedDialogPart("Completed", "Thank you for your input!", group2);
        var welcomePart = new MessageDialogPart("Welcome", "Welcome! I would like to answer a question", group1);
        var dialog2 = new Dialog
        (
            "Dialog2",
            "1.0.0",
            new IDialogPart[] { welcomePart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var redirectPart = new RedirectDialogPart("Redirect", dialog2);
        var dialog1 = new Dialog
        (
            "Dialog1",
            "1.0.0",
            new IDialogPart[] { welcomePart, redirectPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            Enumerable.Empty<IDialogPartGroup>()
        );
        var factory = new DialogContextFactoryFixture(_ => new DialogContextFixture(dialog1));
        var sut = new DialogService(factory);
        var context = sut.Start(dialog1); // this will trigger the message on dialog 1
        context = ((DialogContextFixture)context).WithState(currentState);

        // Act
        var result = sut.Continue(context, Enumerable.Empty<IProvidedAnswer>()); // this will trigger the redirect to dialog 2

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        ((IErrorDialogPart)result.CurrentPart).Exception.Should().NotBeNull();
        ((IErrorDialogPart)result.CurrentPart).Exception!.Message.Should().Be($"Can only continue when the dialog is in progress. Current state is {currentState}");
    }

    [Fact]
    public void Continue_Returns_CompletedDialogPart_When_There_Is_No_Next_DialogPart()
    {
        // Arrange
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var welcomePart = new MessageDialogPart("Welcome", "Welcome! I would like to answer a question", group1);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var completedPart = new CompletedDialogPart("Completed", "Thank you for your input!", group2);
        var dialog = new Dialog
        (
            "Test",
            "1.0.0",
            new IDialogPart[] { welcomePart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var factory = new DialogContextFactoryFixture(_ => new DialogContextFixture(dialog));
        var sut = new DialogService(factory);
        var context = sut.Start(dialog); // this will trigger the message

        // Act
        var result = sut.Continue(context, Enumerable.Empty<IProvidedAnswer>()); // this will trigger the completion

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentPart.Should().BeAssignableTo<ICompletedDialogPart>();
    }

    [Fact]
    public void Start_Uses_Result_From_RedirectPart()
    {
        // Arrange
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var completedPart = new CompletedDialogPart("Completed", "Thank you for your input!", group2);
        var welcomePart = new MessageDialogPart("Welcome", "Welcome! I would like to answer a question", group1);
        var dialog2 = new Dialog
        (
            "Dialog2",
            "1.0.0",
            new IDialogPart[] { welcomePart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var redirectPart = new RedirectDialogPart("Redirect", dialog2);
        var dialog1 = new Dialog
        (
            "Dialog1",
            "1.0.0",
            new[] { redirectPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            Enumerable.Empty<IDialogPartGroup>()
        );
        var factory = new DialogContextFactoryFixture(dialog =>
            dialog.Id == dialog1.Id
                ? new DialogContextFixture(dialog1)
                : new DialogContextFixture(dialog2));
        var sut = new DialogService(factory);

        // Act
        var result = sut.Start(dialog1);

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentDialog.Id.Should().Be(dialog2.Id);
        result.CurrentPart.Id.Should().Be(welcomePart.Id);
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
        var context = new DialogContextFixture(dialog, new ErrorDialogPart("Error", "Not initialized yet", null), null, default, null, Enumerable.Empty<IProvidedAnswer>());
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Start(dialog);

        // Assert
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        errorDialogPart.Exception.Should().NotBeNull();
        errorDialogPart.Exception!.Message.Should().Be("Could not determine next part. Dialog does not have any parts.");
    }

    [Fact]
    public void Start_Returns_First_DialogPart_When_It_Could_Be_Determined()
    {
        // Arrange
        var dialog = CreateDialog();
        var context = new DialogContextFixture(dialog, new ErrorDialogPart("Error", "Not initialized yet", null), null, default, null, Enumerable.Empty<IProvidedAnswer>());
        var factory = new DialogContextFactoryFixture(_ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Start(dialog);

        // Assert
        result.CurrentPart.Should().BeAssignableTo<IMessageDialogPart>();
        result.CurrentPart.Id.Should().Be("Welcome");
    }

    [Fact]
    public void Start_Returns_ErrorDialogPart_When_DecisionPart_Returns_ErrorDialogPart()
    {
        // Arrange
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var completedPart = new CompletedDialogPart("Completed", "Thank you for your input!", group2);
        var decisionPart = new DecisionDialogPartFixture("Decision", (_, _) => errorDialogPart);
        var dialog = new Dialog
        (
            "Test",
            "1.0.0",
            new IDialogPart[] { decisionPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var factory = new DialogContextFactoryFixture(_ => new DialogContextFixture(dialog));
        var sut = new DialogService(factory);

        // Act
        var result = sut.Start(dialog);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        result.CurrentPart.Id.Should().Be(errorDialogPart.Id);
    }

    [Fact]
    public void Start_Returns_AbortDialogPart_When_DecisionPart_Returns_AbortDialogPart()
    {
        // Arrange
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var completedPart = new CompletedDialogPart("Completed", "Thank you for your input!", group2);
        var decisionPart = new DecisionDialogPartFixture("Decision", (_, _) => abortedPart);
        var dialog = new Dialog
        (
            "Test",
            "1.0.0",
            new IDialogPart[] { decisionPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var factory = new DialogContextFactoryFixture(_ => new DialogContextFixture(dialog));
        var sut = new DialogService(factory);

        // Act
        var result = sut.Start(dialog);

        // Assert
        result.CurrentState.Should().Be(DialogState.Aborted);
        result.CurrentPart.Should().BeAssignableTo<IAbortedDialogPart>();
        result.CurrentPart.Id.Should().Be(abortedPart.Id);
    }

    private static Dialog CreateDialog(bool addParts = true)
    {
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var welcomePart = new MessageDialogPart("Welcome", "Welcome! I would like to answer a question", group1);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
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

            if (!values.All(a => a.Question.Id == "Question1"))
            {
                return "Provided answers from wrong question";
            }

            if (!new[] { answerGreat.Id, answerOkay.Id, answerTerrible.Id }.Contains(values.First().Answer.Id))
            {
                return $"Unknown answer: [{values.First().Answer.Id}]";
            }

            // If we've made it up to here, everything is okay! (exactly one valid answer)
            return null;
        });
        var messagePart = new MessageDialogPart("Message", "I'm sorry to hear that. Let us know if we can do something to help you.", group1);
        var completedPart = new CompletedDialogPart("Completed", "Thank you for your input!", group2);
        var decisionPart = new DecisionDialogPartFixture
        (
            "Decision",
            (_, answers) => answers.Any(a => a.Answer.Id == answerTerrible.Id)
                ? messagePart
                : completedPart
        );
        var parts = new IDialogPart[]
        {
            welcomePart,
            questionPart,
            decisionPart,
            messagePart,
            completedPart,
            errorDialogPart,
            abortedPart
        }.Where(_ => addParts);
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
