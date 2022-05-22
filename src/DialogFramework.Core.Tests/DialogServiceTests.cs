namespace DialogFramework.Core.Tests;

public class DialogServiceTests
{
    private static string Id => Guid.NewGuid().ToString();

    [Theory]
    [InlineData(DialogState.Aborted)]
    [InlineData(DialogState.InProgress)]
    public void Abort_Returns_ErrorDialogPart_When_Already_Aborted(DialogState currentState)
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var abortedPart = dialog.AbortedPart;
        var context = new DialogContextFixture(Id, dialog.Metadata, abortedPart, currentState);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

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
    public void Abort_Returns_ErrorDialogPart_When_Already_Completed()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var context = new DialogContextFixture(Id, dialog.Metadata, dialog.CompletedPart, DialogState.Completed);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

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
        var dialog = DialogFixture.CreateDialog();
        var context = new DialogContextFixture(Id, dialog.Metadata, dialog.ErrorPart, DialogState.ErrorOccured);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

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
        var dialog = DialogFixture.CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var abortedPart = dialog.AbortedPart;
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

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
        var dialog = DialogFixture.CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, currentState);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.CanAbort(context);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void CanAbort_Returns_False_When_CurrentPart_Is_AbortedPart()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var abortedPart = dialog.AbortedPart;
        var context = new DialogContextFixture(Id, dialog.Metadata, abortedPart, DialogState.InProgress);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

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
        var dialog = DialogFixture.CreateDialog();
        IDialogPart currentPart = currentState switch
        {
            DialogState.Aborted => dialog.AbortedPart,
            DialogState.Completed => dialog.CompletedPart,
            DialogState.ErrorOccured => dialog.ErrorPart,
            _ => throw new NotImplementedException()
        };
        var context = new DialogContextFixture(Id, dialog.Metadata, currentPart, currentState);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.Continue(context, Enumerable.Empty<IDialogPartResult>());

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
        var dialog = DialogFixture.CreateDialog();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = new DialogContextFixture(Id, dialog.Metadata, currentPart, currentState);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.Continue(context, new[] { new DialogPartResult(currentPart.Id, currentPart.Results.Single(x => x.Id == "Great").Id) });

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentPart.Id.Should().Be("Completed");
        result.CurrentGroup.Should().Be(dialog.CompletedPart.Group);
    }

    [Fact]
    public void Continue_Returns_Same_DialogPart_When_Current_State_Is_Question_And_Answer_Is_Not_Valid()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = new DialogContextFixture(Id, dialog.Metadata, currentPart, currentState);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.Continue(context, new[] { new DialogPartResult(currentPart.Id, "Unknown result") });

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroup.Should().Be(currentPart.Group);
        result.CurrentPart.Should().BeAssignableTo<IQuestionDialogPart>();
        var questionDialogPart = (IQuestionDialogPart)result.CurrentPart;
        questionDialogPart.ValidationErrors.Should().ContainSingle();
        questionDialogPart.ValidationErrors[0].ErrorMessage.Should().Be("Unknown Result Id: [Unknown result]");
    }

    [Fact]
    public void Continue_Returns_Same_DialogPart_On_Answers_From_Wrong_Question()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = new DialogContextFixture(Id, dialog.Metadata, currentPart, currentState);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);
        var wrongQuestionMock = new Mock<IQuestionDialogPart>();
        wrongQuestionMock.SetupGet(x => x.Results).Returns(new ValueCollection<IDialogPartResultDefinition>());

        // Act
        var result = sut.Continue(context, new[] { new DialogPartResult(wrongQuestionMock.Object.Id, "Unknown answer") });

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroup.Should().Be(currentPart.Group);
        result.CurrentPart.Should().BeAssignableTo<QuestionDialogPart>();
        var questionDialogPart = (QuestionDialogPart)result.CurrentPart;
        questionDialogPart.ValidationErrors.Should().ContainSingle();
        questionDialogPart.ValidationErrors[0].ErrorMessage.Should().Be("Provided answer from wrong question");
    }

    [Fact]
    public void Continue_Uses_Result_From_DecisionPart_When_DecisionPart_Returns_No_Error()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.Initial;
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(Id, dialog.Metadata, currentPart, currentState));
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);
        var context = sut.Start(dialog); // start the dialog, this will get the welcome message
        context = sut.Continue(context, Enumerable.Empty<IDialogPartResult>()); // skip the welcome message

        // Act
        var result = sut.Continue(context, new[] { new DialogPartResult(currentPart.Id, currentPart.Results.Single(a => a.Id == "Terrible").Id) }); // answer the question with 'Terrible', this will trigger a second message

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
            new DialogMetadata(
                "Dialog2",
                "Dialog 2",
                "1.0.0",
                true),
            new IDialogPart[] { welcomePart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var redirectPart = new RedirectDialogPart("Redirect", dialog2.Metadata);
        var dialog1 = new Dialog
        (
            new DialogMetadata(
                "Dialog1",
                "Dialog 1",
                "1.0.0",
                true),
            new IDialogPart[] { welcomePart, redirectPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1 }
        );
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog1.Metadata.Id || d.Metadata.Id == dialog2.Metadata.Id,
                                                      d => d.Metadata.Id == dialog1.Metadata.Id
                                                          ? new DialogContextFixture(dialog1.Metadata)
                                                          : new DialogContextFixture(dialog2.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns<IDialogIdentifier>(identifier =>
        {
            if (identifier.Id == dialog1.Metadata.Id && identifier.Version == dialog1.Metadata.Version) return dialog1;
            if (identifier.Id == dialog2.Metadata.Id && identifier.Version == dialog2.Metadata.Version) return dialog2;
            return null;
        });
        var sut = new DialogService(factory, repositoryMock.Object);
        var context = sut.Start(dialog1); // this will trigger the message on dialog 1

        // Act
        var result = sut.Continue(context, Enumerable.Empty<IDialogPartResult>()); // this will trigger the redirect to dialog 2

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentDialogIdentifier.Id.Should().Be(dialog2.Metadata.Id);
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
        var navigationPart = new NavigationDialogPartFixture("Navigate", _ => navigatedPart.Id);
        var dialog = new Dialog
        (
            new DialogMetadata(
                "Test",
                "Test dialog",
                "1.0.0",
                true),
            new IDialogPart[] { welcomePart, navigationPart, navigatedPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1 }
        );
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var sut = new DialogService(factory, repositoryMock.Object);
        var context = sut.Start(dialog); // this will trigger the message

        // Act
        var result = sut.Continue(context, Enumerable.Empty<IDialogPartResult>()); // this will trigger the navigation

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
            new DialogMetadata(
                "Dialog2",
                "Dialog 2",
                "1.0.0",
                true),
            new IDialogPart[] { welcomePart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var redirectPart = new RedirectDialogPart("Redirect", dialog2.Metadata);
        var dialog1 = new Dialog
        (
            new DialogMetadata(
                "Dialog1",
                "Dialog 1",
                "1.0.0",
                true),
            new IDialogPart[] { welcomePart, redirectPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1 }
        );
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog1.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog1.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns<IDialogIdentifier>(identifier =>
        {
            if (identifier.Id == "Dialog1") return dialog1;
            if (identifier.Id == "Dialog2") return dialog2;
            return null;
        });
        var sut = new DialogService(factory, repositoryMock.Object);
        var context = new DialogContextFixture(Id, dialog1.Metadata, dialog1.Parts.First(), currentState);

        // Act
        var result = sut.Continue(context, Enumerable.Empty<IDialogPartResult>()); // this will trigger the redirect to dialog 2

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroup.Should().BeNull();
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        var currentDialogErrorPart = (IErrorDialogPart)result.CurrentPart;
        currentDialogErrorPart.Exception.Should().NotBeNull();
        currentDialogErrorPart.Exception!.Message.Should().Be($"Can only continue when the dialog is in progress. Current state is {currentState}");
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
            new DialogMetadata(
                "Test",
                "Test dialog",
                "1.0.0",
                true),
            new IDialogPart[] { welcomePart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var sut = new DialogService(factory, repositoryMock.Object);
        var context = sut.Start(dialog); // this will trigger the message

        // Act
        var result = sut.Continue(context, Enumerable.Empty<IDialogPartResult>()); // this will trigger the completion

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
        var dialog = DialogFixture.CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, currentState);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.CanContinue(context);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public void CanStart_Returns_Correct_Value_Based_On_Dialog_CanStart(bool dialogCanStart, bool expectedResult)
    {
        // Arrange
        var errorDialogPartMock = new Mock<IErrorDialogPart>();
        var abortedDialogPartMock = new Mock<IAbortedDialogPart>();
        var completedDialogPartMock = new Mock<ICompletedDialogPart>();
        var dialog = new Dialog(new DialogMetadata("Id", "Name", "1.0.0", dialogCanStart),
                                Enumerable.Empty<IDialogPart>(),
                                errorDialogPartMock.Object,
                                abortedDialogPartMock.Object,
                                completedDialogPartMock.Object,
                                Enumerable.Empty<IDialogPartGroup>());
        var contextFactory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                             dialog => new DialogContextFixture(dialog.Metadata));
        var repository = new TestDialogRepository();
        var service = new DialogService(contextFactory, repository);

        // Act
        var actual = service.CanStart(dialog);

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Fact]
    public void CanStart_Returns_False_When_CurrentState_Is_InProgress()
    {
        // Arrange
        var errorDialogPartMock = new Mock<IErrorDialogPart>();
        var abortedDialogPartMock = new Mock<IAbortedDialogPart>();
        var completedDialogPartMock = new Mock<ICompletedDialogPart>();
        var dialog = new Dialog(new DialogMetadata("Id", "Name", "1.0.0", canStart: true), // metadata says we can start
                                Enumerable.Empty<IDialogPart>(),
                                errorDialogPartMock.Object,
                                abortedDialogPartMock.Object,
                                completedDialogPartMock.Object,
                                Enumerable.Empty<IDialogPartGroup>());
        var contextFactory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                             dialog => new DialogContextFixture("Id", dialog.Metadata, errorDialogPartMock.Object, DialogState.InProgress)); // dialog state is already in progress
        var repository = new TestDialogRepository();
        var service = new DialogService(contextFactory, repository);

        // Act
        var actual = service.CanStart(dialog);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void Start_Throws_When_ContextFactory_CanCreate_Returns_False()
    {
        // Arrange
        var factory = new DialogContextFactoryFixture(_ => false,
                                                      _ => throw new InvalidOperationException("Not intended to get to this point"));
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);
        var dialog = DialogFixture.CreateDialog();
        var act = new Action(() => sut.Start(dialog));

        // Act
        act.Should().ThrowExactly<InvalidOperationException>().WithMessage("Could not create context");
    }

    [Fact]
    public void Start_Throws_When_CanStart_Is_False()
    {
        // Arrange
        var dialogMetadataMock = new Mock<IDialogMetadata>();
        dialogMetadataMock.SetupGet(x => x.CanStart).Returns(false);
        var dialogMock = new Mock<IDialog>();
        dialogMock.SetupGet(x => x.Metadata).Returns(dialogMetadataMock.Object);
        var dialogPartMock = new Mock<IDialogPart>();
        var factory = new DialogContextFactoryFixture(_ => true,
                                                      _ => new DialogContextFixture("Id", dialogMock.Object.Metadata, dialogPartMock.Object, DialogState.Initial));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialogMock.Object);
        var sut = new DialogService(factory, repositoryMock.Object);
        var dialog = dialogMock.Object;
        var act = new Action(() => sut.Start(dialog));

        // Act
        act.Should().ThrowExactly<InvalidOperationException>().WithMessage("Could not start dialog");
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
            new DialogMetadata(
                "Dialog2",
                "Dialog 2",
                "1.0.0",
                true),
            new IDialogPart[] { welcomePart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var redirectPart = new RedirectDialogPart("Redirect", dialog2.Metadata);
        var dialog1 = new Dialog
        (
            new DialogMetadata(
                "Dialog1",
                "Dialog 1",
                "1.0.0",
                true),
            new[] { redirectPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            Enumerable.Empty<IDialogPartGroup>()
        );
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog1.Metadata.Id || d.Metadata.Id == dialog2.Metadata.Id,
                                                      dialog => dialog.Metadata.Id == dialog1.Metadata.Id
                                                          ? new DialogContextFixture(dialog1.Metadata)
                                                          : new DialogContextFixture(dialog2.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns<IDialogIdentifier>(identifier =>
        {
            if (identifier.Id == dialog1.Metadata.Id && identifier.Version == dialog1.Metadata.Version) return dialog1;
            if (identifier.Id == dialog2.Metadata.Id && identifier.Version == dialog2.Metadata.Version) return dialog2;
            return null;
        });
        var sut = new DialogService(factory, repositoryMock.Object);

        // Act
        var result = sut.Start(dialog1);

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentDialogIdentifier.Id.Should().Be(dialog2.Metadata.Id);
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
        var navigationPart = new NavigationDialogPartFixture("Navigate", _ => welcomePart.Id);
        var dialog = new Dialog
        (
            new DialogMetadata(
                "Test",
                "Test dialog",
                "1.0.0",
                true),
            new IDialogPart[] { navigationPart, welcomePart },
            errorDialogPart,
            abortedPart,
            completedPart,
            Enumerable.Empty<IDialogPartGroup>()
        );
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

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
        var dialog = DialogFixture.CreateDialog();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => throw new InvalidOperationException("Kaboom"));
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);
        var start = new Action(() => sut.Start(dialog));

        // Act
        start.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    [Fact]
    public void Start_Returns_ErrorDialogPart_When_First_DialogPart_Could_Not_Be_Determined()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog(false);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

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
        var dialog = DialogFixture.CreateDialog();
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

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
        var decisionPart = new DecisionDialogPartFixture("Decision", (_, _) => errorDialogPart.Id);
        var dialog = new Dialog
        (
            new DialogMetadata(
                "Test",
                "Test dialog",
                "1.0.0",
                true),
            new IDialogPart[] { decisionPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

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
        var decisionPart = new DecisionDialogPartFixture("Decision", (_, _) => abortedPart.Id);
        var dialog = new Dialog
        (
            new DialogMetadata(
                "Test",
                "Test dialog",
                "1.0.0",
                true),
            new IDialogPart[] { decisionPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.Start(dialog);

        // Assert
        result.CurrentState.Should().Be(DialogState.Aborted);
        result.CurrentGroup.Should().BeNull();
        result.CurrentPart.Should().BeAssignableTo<IAbortedDialogPart>();
        result.CurrentPart.Id.Should().Be(abortedPart.Id);
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Parts_Does_Not_Contain_Current_Part()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var completedPart = dialog.CompletedPart;
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.CanNavigateTo(context, completedPart);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Requested_Part_Is_Not_Part_Of_Current_Dialog()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var errorPart = dialog.ErrorPart;
        var completedPart = dialog.CompletedPart;
        var context = new DialogContextFixture(Id, dialog.Metadata, errorPart, DialogState.ErrorOccured);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.CanNavigateTo(context, completedPart);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Requested_Part_Is_After_Current_Part()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, messagePart, DialogState.InProgress);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.CanNavigateTo(context, questionPart);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_Part_Is_Current_Part()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.CanNavigateTo(context, questionPart);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_Part_Is_Before_Current_Part()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        IDialogContext context = new DialogContextFixture(Id, dialog.Metadata, messagePart, DialogState.InProgress);
        context = context.AddDialogPartResults(new[] { new DialogPartResult(messagePart.Id) }, dialog);
        context = context.Continue(questionPart, DialogState.InProgress);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.CanNavigateTo(context, messagePart);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void NavigateTo_Throws_When_CanNavigateTo_Is_False()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, messagePart, DialogState.InProgress);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);
        var navigateTo = new Action(() => sut.NavigateTo(context, questionPart));

        // Act & Assert
        navigateTo.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NavigateTo_Navigates_To_Requested_Part_When_CanNavigate_Is_True()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        IDialogContext context = new DialogContextFixture(Id, dialog.Metadata, messagePart, DialogState.InProgress);
        context = context.AddDialogPartResults(new[] { new DialogPartResult(messagePart.Id) }, dialog);
        context = context.Continue(questionPart, DialogState.InProgress);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.NavigateTo(context, messagePart);

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroup.Should().Be(messagePart.Group);
        result.CurrentPart.Should().BeAssignableTo<IMessageDialogPart>();
        result.CurrentPart.Id.Should().Be(messagePart.Id);
    }

    [Theory]
    [InlineData(DialogState.Aborted)]
    [InlineData(DialogState.Completed)]
    [InlineData(DialogState.ErrorOccured)]
    [InlineData(DialogState.Initial)]
    public void CanResetCurrentState_Returns_False_When_CurrentState_Is(DialogState currentState)
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, currentState);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.CanResetCurrentState(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanResetCurrentState_Returns_False_When_Current_DialogPart_Is_Not_QuestionDialogPart()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var context = new DialogContextFixture(Id, dialog.Metadata, dialog.CompletedPart, DialogState.InProgress); // note that this actually invalid state, but we currently can't prevent it on the interface
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.CanResetCurrentState(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanResetCurrentState_Returns_True_When_CurrentState_Is_InProgress_And_Current_DialogPart_Is_QuestionDialogPart()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.CanResetCurrentState(context);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ResetCurrentState_Resets_Answers_From_Current_Question()
    {
        // Arrange
        var dialog = DialogFixture.CreateDialog();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        context.AddAnswer(new DialogPartResult(questionPart.Id, "Terrible"));
        context.AddAnswer(new DialogPartResult("Other part", "Other value"));
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var sut = new DialogService(factory, repository);

        // Act
        var result = sut.ResetCurrentState(context);

        // Assert
        var dialogPartResults = result.GetAllDialogPartResults();
        dialogPartResults.Should().ContainSingle();
        dialogPartResults.Single().DialogPartId.Should().Be("Other part");
    }
}
