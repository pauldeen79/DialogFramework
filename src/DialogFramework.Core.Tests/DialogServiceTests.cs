namespace DialogFramework.Core.Tests;

public class DialogServiceTests
{
    [Theory]
    [InlineData(DialogState.Aborted)]
    [InlineData(DialogState.InProgress)]
    public void Abort_Returns_ErrorDialogPart_When_Already_Aborted(DialogState currentState)
    {
        // Arrange
        var dialog = CreateDialog();
        var abortedPart = dialog.AbortedPart;
        var context = new DialogContextFixture(dialog, abortedPart, currentState);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroup.Should().BeNull();
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        errorDialogPart.Exception.Should().NotBeNull();
        errorDialogPart.Exception!.Message.Should().Be("Dialog has already been aborted");
    }

    [Fact]
    public void Abort_Returns_ErrorDialogPart_When_Already_Completed()
    {
        // Arrange
        var dialog = CreateDialog();
        var context = new DialogContextFixture(dialog, dialog.CompletedPart, DialogState.Completed);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroup.Should().BeNull();
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        errorDialogPart.Exception.Should().NotBeNull();
        errorDialogPart.Exception!.Message.Should().Be("Dialog cannot be aborted");
    }

    [Fact]
    public void Abort_Returns_ErrorDialogPart_When_Dialog_Is_In_ErrorState()
    {
        // Arrange
        var dialog = CreateDialog();
        var context = new DialogContextFixture(dialog, dialog.ErrorPart, DialogState.ErrorOccured);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroup.Should().BeNull();
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        errorDialogPart.Exception.Should().NotBeNull();
        errorDialogPart.Exception!.Message.Should().Be("Dialog cannot be aborted");
    }

    [Fact]
    public void Abort_Returns_AbortDialogPart_Dialog_When_Dialog_Is_InProgress()
    {
        // Arrange
        var dialog = CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var abortedPart = dialog.AbortedPart;
        var context = new DialogContextFixture(dialog, questionPart, DialogState.InProgress);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.Aborted);
        result.CurrentPart.Should().Be(abortedPart);
        result.CurrentGroup.Should().BeNull();
    }

    [Theory]
    [InlineData(DialogState.Aborted, false)]
    [InlineData(DialogState.Completed, false)]
    [InlineData(DialogState.ErrorOccured, false)]
    [InlineData(DialogState.Initial, false)]
    [InlineData(DialogState.InProgress, true)]
    public void CanAbort_Returns_Correct_Result_Based_On_Current_State(DialogState currentState, bool expectedResult)
    {
        // Arrange
        var dialog = CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(dialog, questionPart, currentState);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.CanAbort(context);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void CanAbort_Returns_False_When_CurrentPart_Is_AbortedPart()
    {
        // Arrange
        var dialog = CreateDialog();
        var abortedPart = dialog.AbortedPart;
        var context = new DialogContextFixture(dialog, abortedPart, DialogState.InProgress);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.CanAbort(context);

        // Assert
        result.Should().BeFalse();
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
            DialogState.Aborted => dialog.AbortedPart,
            DialogState.Completed => dialog.CompletedPart,
            DialogState.ErrorOccured => dialog.ErrorPart,
            _ => throw new NotImplementedException()
        };
        var context = new DialogContextFixture(dialog, currentPart, currentState);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Continue(context, Enumerable.Empty<IProvidedAnswer>());

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroup.Should().BeNull();
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
        var context = new DialogContextFixture(dialog, currentPart, currentState);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Continue(context, new[] { new ProvidedAnswer(currentPart, currentPart.Answers.Single(x => x.Id == "Great")) });

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentPart.Id.Should().Be("Completed");
        result.CurrentGroup.Should().Be(dialog.CompletedPart.Group);
    }

    [Fact]
    public void Continue_Returns_Same_DialogPart_When_Current_State_Is_Question_And_Answer_Is_Not_Valid()
    {
        // Arrange
        var dialog = CreateDialog();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = new DialogContextFixture(dialog, currentPart, currentState);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);
        var answerMock = new Mock<IQuestionDialogPartAnswer>();
        answerMock.SetupGet(x => x.Id).Returns("Unknown answer");

        // Act
        var result = sut.Continue(context, new[] { new ProvidedAnswer(currentPart, answerMock.Object) });

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroup.Should().Be(currentPart.Group);
        result.CurrentPart.Should().BeAssignableTo<QuestionDialogPartFixture>();
        var questionDialogPart = (QuestionDialogPartFixture)result.CurrentPart;
        questionDialogPart.ErrorMessages.Should().ContainSingle();
        questionDialogPart.ErrorMessages[0].Should().Be("Unknown answer: [Unknown answer]");
    }

    [Fact]
    public void Continue_Returns_Same_DialogPart_On_Answers_From_Wrong_Question()
    {
        // Arrange
        var dialog = CreateDialog();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = new DialogContextFixture(dialog, currentPart, currentState);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);
        var answerMock = new Mock<IQuestionDialogPartAnswer>();
        answerMock.SetupGet(x => x.Id).Returns("Unknown answer");
        var wrongQuestionMock = new Mock<IQuestionDialogPart>();
        wrongQuestionMock.SetupGet(x => x.Answers).Returns(new ValueCollection<IQuestionDialogPartAnswer>());

        // Act
        var result = sut.Continue(context, new[] { new ProvidedAnswer(wrongQuestionMock.Object, answerMock.Object) });

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroup.Should().Be(currentPart.Group);
        result.CurrentPart.Should().BeAssignableTo<QuestionDialogPartFixture>();
        var questionDialogPart = (QuestionDialogPartFixture)result.CurrentPart;
        questionDialogPart.ErrorMessages.Should().ContainSingle();
        questionDialogPart.ErrorMessages[0].Should().Be("Provided answer from wrong question");
    }

    [Fact]
    public void Continue_Uses_Result_From_DecisionPart_When_DecisionPart_Returns_No_Error()
    {
        // Arrange
        var dialog = CreateDialog();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => new DialogContextFixture(dialog, currentPart, currentState));
        var sut = new DialogService(factory);
        var context = sut.Start(dialog); // start the dialog, this will get the welcome message
        context = sut.Continue(context, Enumerable.Empty<IProvidedAnswer>()); // skip the welcome message

        // Act
        var result = sut.Continue(context, new[] { new ProvidedAnswer(currentPart, currentPart.Answers.Single(a => a.Id == "Terrible")) }); // answer the question with 'Terrible', this will trigger a second message

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentPart.Id.Should().Be("Message");
        result.CurrentGroup.Should().Be(dialog.Parts.OfType<IMessageDialogPart>().First(x => x.Id == "Message").Group);
    }

    [Fact]
    public void Continue_Uses_Result_From_RedirectPart()
    {
        // Arrange
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var completedPart = new CompletedDialogPart("Completed", "Thank you", "Thank you for your input!", group2);
        var welcomePart = new MessageDialogPart("Welcome", "Welcome", "Welcome! I would like to answer a question", group1);
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
            new[] { group1 }
        );
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog1.Id || d.Id == dialog2.Id, d =>
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
        result.CurrentGroup.Should().Be(welcomePart.Group);
        result.CurrentPart.Id.Should().Be(welcomePart.Id);
    }

    [Fact]
    public void Continue_Uses_Result_From_NavigationPart()
    {
        // Arrange
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var completedPart = new CompletedDialogPart("Completed", "Thank you", "Thank you for your input!", group2);
        var welcomePart = new MessageDialogPart("Welcome", "Welcome", "Welcome! I would like to answer a question", group1);
        var navigatedPart = new MessageDialogPart("Navigated", "Navigated", "This shows that navigation works", group2);
        var navigationPart = new NavigationDialogPartFixture("Navigate", _ => navigatedPart);
        var dialog = new Dialog
        (
            "Test",
            "1.0.0",
            new IDialogPart[] { welcomePart, navigationPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1 }
        );
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => new DialogContextFixture(dialog));
        var sut = new DialogService(factory);
        var context = sut.Start(dialog); // this will trigger the message

        // Act
        var result = sut.Continue(context, Enumerable.Empty<IProvidedAnswer>()); // this will trigger the navigation

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroup.Should().Be(navigatedPart.Group);
        result.CurrentPart.Id.Should().Be(navigatedPart.Id);
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
        var completedPart = new CompletedDialogPart("Completed", "Completed", "Thank you for your input!", group2);
        var welcomePart = new MessageDialogPart("Welcome", "Welcome", "Welcome! I would like to answer a question", group1);
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
            new[] { group1 }
        );
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog1.Id, _ => new DialogContextFixture(dialog1));
        var sut = new DialogService(factory);
        var context = sut.Start(dialog1); // this will trigger the message on dialog 1
        context = ((DialogContextFixture)context).WithState(currentState);

        // Act
        var result = sut.Continue(context, Enumerable.Empty<IProvidedAnswer>()); // this will trigger the redirect to dialog 2

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroup.Should().BeNull();
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
        var welcomePart = new MessageDialogPart("Welcome", "Welcome", "Welcome! I would like to answer a question", group1);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var completedPart = new CompletedDialogPart("Completed", "Completed", "Thank you for your input!", group2);
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
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => new DialogContextFixture(dialog));
        var sut = new DialogService(factory);
        var context = sut.Start(dialog); // this will trigger the message

        // Act
        var result = sut.Continue(context, Enumerable.Empty<IProvidedAnswer>()); // this will trigger the completion

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentGroup.Should().Be(completedPart.Group);
        result.CurrentPart.Should().BeAssignableTo<ICompletedDialogPart>();
    }

    [Theory]
    [InlineData(DialogState.Aborted, false)]
    [InlineData(DialogState.Completed, false)]
    [InlineData(DialogState.ErrorOccured, false)]
    [InlineData(DialogState.Initial, false)]
    [InlineData(DialogState.InProgress, true)]
    public void CanContinue_Returns_Correct_Result_Based_On_Current_State(DialogState currentState, bool expectedResult)
    {
        // Arrange
        var dialog = CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(dialog, questionPart, currentState);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.CanContinue(context);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void Start_Throws_When_ContextFactory_CanCreate_Returns_False()
    {
        // Arrange
        var factory = new DialogContextFactoryFixture(_ => false, _ => throw new InvalidOperationException("Not intended to get to this point"));
        var sut = new DialogService(factory);
        var dialog = CreateDialog();
        var act = new Action(() => sut.Start(dialog));

        // Act
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Start_Uses_Result_From_RedirectPart()
    {
        // Arrange
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var completedPart = new CompletedDialogPart("Completed", "Completed", "Thank you for your input!", group2);
        var welcomePart = new MessageDialogPart("Welcome", "Welcome", "Welcome! I would like to answer a question", group1);
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
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog1.Id || d.Id == dialog2.Id, dialog =>
            dialog.Id == dialog1.Id
                ? new DialogContextFixture(dialog1)
                : new DialogContextFixture(dialog2));
        var sut = new DialogService(factory);

        // Act
        var result = sut.Start(dialog1);

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentDialog.Id.Should().Be(dialog2.Id);
        result.CurrentGroup.Should().Be(welcomePart.Group);
        result.CurrentPart.Id.Should().Be(welcomePart.Id);
    }

    [Fact]
    public void Start_Uses_Result_From_NavigationPart()
    {
        // Arrange
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var completedPart = new CompletedDialogPart("Completed", "Completed", "Thank you for your input!", group2);
        var welcomePart = new MessageDialogPart("Welcome", "Welcome", "Welcome! I would like to answer a question", group1);
        var navigationPart = new NavigationDialogPartFixture("Navigate", _ => welcomePart);
        var dialog = new Dialog
        (
            "Test",
            "1.0.0",
            new[] { navigationPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            Enumerable.Empty<IDialogPartGroup>()
        );
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => new DialogContextFixture(dialog));
        var sut = new DialogService(factory);

        // Act
        var result = sut.Start(dialog);

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroup.Should().Be(welcomePart.Group);
        result.CurrentPart.Id.Should().Be(welcomePart.Id);
    }

    [Fact]
    public void Start_Throws_When_Context_Could_Not_Be_Created()
    {
        // Arrange
        var dialog = CreateDialog();
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => throw new InvalidOperationException("Kaboom"));
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
        var context = new DialogContextFixture(dialog);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Start(dialog);

        // Assert
        result.CurrentGroup.Should().BeNull();
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
        var context = new DialogContextFixture(dialog);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.Start(dialog);

        // Assert
        result.CurrentGroup.Should().Be(dialog.Parts.OfType<IMessageDialogPart>().First().Group);
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
        var completedPart = new CompletedDialogPart("Completed", "Completed", "Thank you for your input!", group2);
        var decisionPart = new DecisionDialogPartFixture("Decision", _ => errorDialogPart);
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
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => new DialogContextFixture(dialog));
        var sut = new DialogService(factory);

        // Act
        var result = sut.Start(dialog);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroup.Should().BeNull();
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
        var completedPart = new CompletedDialogPart("Completed", "Completed", "Thank you for your input!", group2);
        var decisionPart = new DecisionDialogPartFixture("Decision", _ => abortedPart);
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
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => new DialogContextFixture(dialog));
        var sut = new DialogService(factory);

        // Act
        var result = sut.Start(dialog);

        // Assert
        result.CurrentState.Should().Be(DialogState.Aborted);
        result.CurrentGroup.Should().BeNull();
        result.CurrentPart.Should().BeAssignableTo<IAbortedDialogPart>();
        result.CurrentPart.Id.Should().Be(abortedPart.Id);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public void CanStart_Returns_Correct_Result_Based_On_ContextFactory_CanCreate_Result(bool contextFactoryCanCreate, bool expectedResult)
    {
        // Arrange
        var factory = new DialogContextFactoryFixture(_ => contextFactoryCanCreate, _ => throw new InvalidOperationException("Not intended to get to this point"));
        var sut = new DialogService(factory);
        var dialog = CreateDialog();

        // Act
        var result = sut.CanStart(dialog);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Parts_Does_Not_Contain_Current_Part()
    {
        // Arrange
        var dialog = CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var completedPart = dialog.CompletedPart;
        var context = new DialogContextFixture(dialog, questionPart, DialogState.InProgress);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.CanNavigateTo(context, completedPart);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Requested_Part_Is_Not_Part_Of_Current_Dialog()
    {
        // Arrange
        var dialog = CreateDialog();
        var errorPart = dialog.ErrorPart;
        var completedPart = dialog.CompletedPart;
        var context = new DialogContextFixture(dialog, errorPart, DialogState.ErrorOccured);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.CanNavigateTo(context, completedPart);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Requested_Part_Is_After_Current_Part()
    {
        // Arrange
        var dialog = CreateDialog();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(dialog, messagePart, DialogState.InProgress);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.CanNavigateTo(context, questionPart);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_Part_Is_Current_Part()
    {
        // Arrange
        var dialog = CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(dialog, questionPart, DialogState.InProgress);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.CanNavigateTo(context, questionPart);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_Part_Is_Before_Current_Part()
    {
        // Arrange
        var dialog = CreateDialog();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(dialog, questionPart, DialogState.InProgress);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.CanNavigateTo(context, messagePart);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void NavigateTo_Throws_When_CanNavigateTo_Is_False()
    {
        // Arrange
        var dialog = CreateDialog();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(dialog, messagePart, DialogState.InProgress);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);
        var navigateTo = new Action(() => sut.NavigateTo(context, questionPart));

        // Act & Assert
        navigateTo.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NavigateTo_Navigates_To_Requested_Part_When_CanNavigate_Is_True()
    {
        // Arrange
        var dialog = CreateDialog();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(dialog, questionPart, DialogState.InProgress);
        var factory = new DialogContextFactoryFixture(d => d.Id == dialog.Id, _ => context);
        var sut = new DialogService(factory);

        // Act
        var result = sut.NavigateTo(context, messagePart);

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroup.Should().Be(messagePart.Group);
        result.CurrentPart.Should().BeAssignableTo<IMessageDialogPart>();
        result.CurrentPart.Id.Should().Be(messagePart.Id);
    }

    private static Dialog CreateDialog(bool addParts = true)
    {
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var welcomePart = new MessageDialogPart("Welcome", "Welcome", "Welcome! I would like to answer a question", group1);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var answerGreat = new QuestionDialogPartAnswerFixture("Great", "I feel great, thank you!", AnswerValueType.None);
        var answerOkay = new QuestionDialogPartAnswerFixture("Okay", "I feel kind of okay", AnswerValueType.None);
        var answerTerrible = new QuestionDialogPartAnswerFixture("Terrible", "I feel terrible, don't want to talk about it", AnswerValueType.None);
        var questionPart = new QuestionDialogPartFixture("Question1", "How do you feel", "Please tell us how you feel", group1, new[] { answerGreat, answerOkay, answerTerrible });
        var messagePart = new MessageDialogPart("Message", "Message", "I'm sorry to hear that. Let us know if we can do something to help you.", group1);
        var completedPart = new CompletedDialogPart("Completed", "Completed", "Thank you for your input!", group2);
        var decisionPart = new DecisionDialogPartFixture
        (
            "Decision",
            ctx => ((DialogContextFixture)ctx).Answers.Any(a => a.Question.Id == questionPart.Id && a.Answer.Id == answerTerrible.Id)
                ? messagePart
                : completedPart
        );
        var parts = new IDialogPart[]
        {
            welcomePart,
            questionPart,
            decisionPart,
            messagePart
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
