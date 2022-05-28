namespace DialogFramework.Application.Tests;

public class DialogServiceTests
{
    private static string Id => Guid.NewGuid().ToString();

    [Theory]
    [InlineData(DialogState.Aborted)]
    [InlineData(DialogState.InProgress)]
    public void Abort_Returns_ErrorDialogPart_When_Already_Aborted(DialogState currentState)
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var abortedPart = dialog.AbortedPart;
        var context = new DialogContextFixture(Id, dialog.Metadata, abortedPart, currentState);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroup.Should().BeNull();
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        //var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        //errorDialogPart.Exception.Should().NotBeNull();
        //errorDialogPart.Exception!.Message.Should().Be("Dialog cannot be aborted");
    }

    [Fact]
    public void Abort_Returns_ErrorDialogPart_When_Already_Completed()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture(Id, dialog.Metadata, dialog.CompletedPart, DialogState.Completed);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroup.Should().BeNull();
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        //var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        //errorDialogPart.Exception.Should().NotBeNull();
        //errorDialogPart.Exception!.Message.Should().Be("Dialog cannot be aborted");
    }

    [Fact]
    public void Abort_Returns_ErrorDialogPart_When_Dialog_Is_In_ErrorState()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture(Id, dialog.Metadata, dialog.ErrorPart, DialogState.ErrorOccured);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroup.Should().BeNull();
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        //var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        //errorDialogPart.Exception.Should().NotBeNull();
        //errorDialogPart.Exception!.Message.Should().Be("Dialog cannot be aborted");
    }

    [Fact]
    public void Abort_Returns_AbortDialogPart_Dialog_When_Dialog_Is_InProgress()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var abortedPart = dialog.AbortedPart;
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.Aborted);
        result.CurrentPart.Should().Be(abortedPart);
        result.CurrentGroup.Should().BeNull();
    }

    [Fact]
    public void Abort_Throws_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var abort = new Action(() => sut.Abort(factory.Create(dialog)));

        // Act
        abort.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog: Id [DialogFixture], Version [1.0.0]");
    }

    [Fact]
    public void Abort_Throws_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var abort = new Action(() => sut.Abort(factory.Create(dialog)));

        // Act
        abort.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
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
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, currentState);
        var sut = CreateSut();

        // Act
        var result = sut.CanAbort(context);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void CanAbort_Returns_False_When_CurrentPart_Is_AbortedPart()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var abortedPart = dialog.AbortedPart;
        var context = new DialogContextFixture(Id, dialog.Metadata, abortedPart, DialogState.InProgress);
        var sut = CreateSut();

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
        var dialog = DialogFixture.CreateBuilder().Build();
        IDialogPart currentPart = currentState switch
        {
            DialogState.Aborted => dialog.AbortedPart,
            DialogState.Completed => dialog.CompletedPart,
            DialogState.ErrorOccured => dialog.ErrorPart,
            _ => throw new NotImplementedException()
        };
        var context = new DialogContextFixture(Id, dialog.Metadata, currentPart, currentState);
        var sut = CreateSut();

        // Act
        var result = sut.Continue(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroup.Should().BeNull();
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        //var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        //errorDialogPart.Exception.Should().NotBeNull();
        //errorDialogPart.Exception!.Message.Should().Be($"Can only continue when the dialog is in progress. Current state is {currentState}");
    }

    [Fact]
    public void Continue_Returns_Next_DialogPart_When_Current_State_Is_Question_And_Answer_Is_Valid()
    {
        // Arrange
        var dialog = DialogFixture.CreateHowDoYouFeelBuilder().Build();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = new DialogContextFixture(Id, dialog.Metadata, currentPart, currentState);
        var sut = CreateSut();
        var dialogPartResult = new DialogPartResultBuilder()
            .WithDialogPartId(currentPart.Id)
            .WithResultId("Great")
            .Build();

        // Act
        var result = sut.Continue(context, new[] { dialogPartResult });

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentPart.Id.Should().Be("Completed");
        result.CurrentGroup.Should().Be(dialog.CompletedPart.Group);
    }

    [Fact]
    public void Continue_Returns_Same_DialogPart_When_Current_State_Is_Question_And_Answer_Is_Not_Valid()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = new DialogContextFixture(Id, dialog.Metadata, currentPart, currentState);
        var sut = CreateSut();
        var dialogPartResult = new DialogPartResultBuilder()
            .WithDialogPartId(currentPart.Id)
            .WithResultId("Unknown result")
            .Build();

        // Act
        var result = sut.Continue(context, new[] { dialogPartResult });

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroup.Should().Be(currentPart.Group);
        result.CurrentPart.Should().BeAssignableTo<IQuestionDialogPart>();
        var questionDialogPart = (IQuestionDialogPart)result.CurrentPart;
        questionDialogPart.ValidationErrors.Should().ContainSingle();
        questionDialogPart.ValidationErrors.Single().ErrorMessage.Should().Be("Unknown Result Id: [Unknown result]");
    }

    [Fact]
    public void Continue_Returns_Same_DialogPart_On_Answers_From_Wrong_Question()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = new DialogContextFixture(Id, dialog.Metadata, currentPart, currentState);
        var sut = CreateSut();
        var wrongQuestionMock = new Mock<IQuestionDialogPart>();
        wrongQuestionMock.SetupGet(x => x.Results).Returns(new ReadOnlyValueCollection<IDialogPartResultDefinition>());
        var dialogPartResult = new DialogPartResultBuilder()
            .WithDialogPartId(wrongQuestionMock.Object.Id)
            .WithResultId("Unknown answer")
            .Build();

        // Act
        var result = sut.Continue(context, new[] { dialogPartResult });

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroup.Should().Be(currentPart.Group);
        result.CurrentPart.Should().BeAssignableTo<QuestionDialogPart>();
        var questionDialogPart = (QuestionDialogPart)result.CurrentPart;
        questionDialogPart.ValidationErrors.Should().ContainSingle();
        questionDialogPart.ValidationErrors.Single().ErrorMessage.Should().Be("Provided answer from wrong question");
    }

    [Fact]
    public void Continue_Uses_Result_From_DecisionPart_When_DecisionPart_Returns_No_Error()
    {
        // Arrange
        var dialog = DialogFixture.CreateHowDoYouFeelBuilder().Build();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.Initial;
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(Id, dialog.Metadata, currentPart, currentState));
        var repository = new TestDialogRepository();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repository, conditionEvaluator);
        var context = sut.Start(dialog.Metadata); // start the dialog, this will get the welcome message
        context = sut.Continue(context); // skip the welcome message
        var dialogPartResult = new DialogPartResultBuilder()
            .WithDialogPartId(currentPart.Id)
            .WithResultId("Terrible")
            .Build();

        // Act
        var result = sut.Continue(context, new[] { dialogPartResult }); // answer the question with 'Terrible', this will trigger a second message

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentPart.Id.Should().Be("Completed");
        result.CurrentGroup.Should().Be(dialog.CompletedPart.Group);
    }

    [Fact]
    public void Continue_Uses_Result_From_RedirectPart()
    {
        // Arrange
        var group1 = new DialogPartGroupBuilder()
            .WithId("Part1")
            .WithTitle("Give information")
            .WithNumber(1);
        var group2 = new DialogPartGroupBuilder()
            .WithId("Part2")
            .WithTitle("Completed")
            .WithNumber(2);
        var errorDialogPart = new ErrorDialogPartBuilder()
            .WithErrorMessage("Something went horribly wrong!")
            .WithId("Error");
        var abortedPart = new AbortedDialogPartBuilder()
            .WithMessage("Dialog has been aborted")
            .WithId("Abort");
        var completedPart = new CompletedDialogPartBuilder()
            .WithMessage("Thank you for your input!")
            .WithGroup(group2)
            .WithHeading("Thank you")
            .WithId("Completed");
        var welcomePart = new MessageDialogPartBuilder()
            .WithMessage("Welcome! I would like to answer a question")
            .WithGroup(group1)
            .WithHeading("Welcome")
            .WithId( "Welcome");
        var dialog2 = new DialogBuilder()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 2")
                .WithId("Dialog2")
                .WithVersion("1.0.0"))
            .AddParts(new DialogPartBuilder(welcomePart))
            .WithErrorPart(errorDialogPart)
            .WithAbortedPart(abortedPart)
            .WithCompletedPart(completedPart)
            .AddPartGroups(group1, group2)
            .Build();
        var redirectPart = new RedirectDialogPartBuilder()
            .WithRedirectDialogMetadata(new DialogMetadataBuilder(dialog2.Metadata))
            .WithId("Redirect");
        var dialog1 = new DialogBuilder()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 1")
                .WithId("Dialog1")
                .WithVersion("1.0.0"))
            .AddParts
            (
                new DialogPartBuilder(welcomePart),
                new DialogPartBuilder(redirectPart)
            )
            .WithErrorPart(errorDialogPart)
            .WithAbortedPart(abortedPart)
            .WithCompletedPart(completedPart)
            .AddPartGroups(group1)
            .Build();
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
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var context = sut.Start(dialog1.Metadata); // this will trigger the message on dialog 1

        // Act
        var result = sut.Continue(context); // this will trigger the redirect to dialog 2

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentDialogIdentifier.Id.Should().Be(dialog2.Metadata.Id);
        result.CurrentGroup.Should().Be(welcomePart.Group.Build());
        result.CurrentPart.Id.Should().Be(welcomePart.Id);
    }

    [Fact]
    public void Continue_Uses_Result_From_NavigationPart()
    {
        // Arrange
        var group1 = new DialogPartGroupBuilder()
            .WithId("Part1")
            .WithTitle("Give information")
            .WithNumber(1);
        var group2 = new DialogPartGroupBuilder()
            .WithId("Part2")
            .WithTitle("Completed")
            .WithNumber(2);
        var errorDialogPart = new ErrorDialogPartBuilder()
            .WithErrorMessage("Something went horribly wrong!")
            .WithId("Error");
        var abortedPart = new AbortedDialogPartBuilder()
            .WithMessage("Dialog has been aborted")
            .WithId("Abort");
        var completedPart = new CompletedDialogPartBuilder()
            .WithMessage("Thank you for your input!")
            .WithGroup(group2)
            .WithHeading("Thank you")
            .WithId("Completed");
        var welcomePart = new MessageDialogPartBuilder()
            .WithMessage("Welcome! I would like to answer a question")
            .WithGroup(group1)
            .WithHeading("Welcome")
            .WithId("Welcome");
        var navigatedPart = new MessageDialogPartBuilder()
            .WithMessage("This shows that navigation works")
            .WithGroup(group2)
            .WithHeading("Navigated")
            .WithId("Navigated");
        var navigationPart = new NavigationDialogPartBuilder()
            .WithId("Navigate")
            .WithNavigateToId(navigatedPart.Id);
        var dialog = new DialogBuilder()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Test dialog")
                .WithId("Test")
                .WithVersion("1.0.0"))
            .AddParts
            (
                new DialogPartBuilder(welcomePart),
                new DialogPartBuilder(navigationPart),
                new DialogPartBuilder(navigatedPart)
            )
            .WithErrorPart(errorDialogPart)
            .WithAbortedPart(abortedPart)
            .WithCompletedPart(completedPart)
            .AddPartGroups(group1)
            .Build();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var context = sut.Start(dialog.Metadata); // this will trigger the message

        // Act
        var result = sut.Continue(context); // this will trigger the navigation

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroup.Should().Be(navigatedPart.Group.Build());
        result.CurrentPart.Id.Should().Be(navigatedPart.Id);
    }

    [Theory]
    [InlineData(DialogState.Aborted)]
    [InlineData(DialogState.Completed)]
    [InlineData(DialogState.ErrorOccured)]
    public void Continue_Returns_ErrorDialogPart_When_State_Is_Wrong(DialogState currentState)
    {
        // Arrange
        var sut = CreateSutForTwoDialogsWithRedirect(out var dialog1);
        var context = new DialogContextFixture(Id, dialog1.Metadata, dialog1.Parts.First(), currentState);

        // Act
        var result = sut.Continue(context); // this will trigger the redirect to dialog 2

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroup.Should().BeNull();
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        //var currentDialogErrorPart = (IErrorDialogPart)result.CurrentPart;
        //currentDialogErrorPart.Exception.Should().NotBeNull();
        //currentDialogErrorPart.Exception!.Message.Should().Be($"Can only continue when the dialog is in progress. Current state is {currentState}");
    }

    [Fact]
    public void Continue_Returns_CompletedDialogPart_When_There_Is_No_Next_DialogPart()
    {
        // Arrange
        var group1 = new DialogPartGroupBuilder()
            .WithId("Part1")
            .WithTitle("Give information")
            .WithNumber(1);
        var group2 = new DialogPartGroupBuilder()
            .WithId("Part2")
            .WithTitle("Completed")
            .WithNumber(2);
        var errorDialogPart = new ErrorDialogPartBuilder()
            .WithErrorMessage("Something went horribly wrong!")
            .WithId("Error");
        var abortedPart = new AbortedDialogPartBuilder()
            .WithMessage("Dialog has been aborted")
            .WithId("Abort");
        var completedPart = new CompletedDialogPartBuilder()
            .WithMessage("Thank you for your input!")
            .WithGroup(group2)
            .WithHeading("Thank you")
            .WithId("Completed");
        var welcomePart = new MessageDialogPartBuilder()
            .WithMessage("Welcome! I would like to answer a question")
            .WithGroup(group1)
            .WithHeading("Welcome")
            .WithId("Welcome");
        var dialog = new DialogBuilder()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Test dialog")
                .WithId("Test")
                .WithVersion("1.0.0"))
            .AddParts(new DialogPartBuilder(welcomePart))
            .WithErrorPart(errorDialogPart)
            .WithAbortedPart(abortedPart)
            .WithCompletedPart(completedPart)
            .AddPartGroups(group1, group2)
            .Build();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var context = sut.Start(dialog.Metadata); // this will trigger the message

        // Act
        var result = sut.Continue(context); // this will trigger the completion

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentGroup.Should().Be(completedPart.Group.Build());
        result.CurrentPart.Should().BeAssignableTo<ICompletedDialogPart>();
    }

    [Fact]
    public void Continue_Throws_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var continuation = new Action(() => sut.Continue(factory.Create(dialog)));

        // Act
        continuation.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog: Id [DialogFixture], Version [1.0.0]");
    }

    [Fact]
    public void Continue_Throws_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var continuation = new Action(() => sut.Continue(factory.Create(dialog)));

        // Act
        continuation.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
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
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, currentState);
        var sut = CreateSut();

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
        var dialog = new Dialog(new DialogMetadata("Name", dialogCanStart, "Id", "1.0.0"),
                                Enumerable.Empty<IDialogPart>(),
                                errorDialogPartMock.Object,
                                abortedDialogPartMock.Object,
                                completedDialogPartMock.Object,
                                Enumerable.Empty<IDialogPartGroup>());
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      dialog => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);

        // Act
        var actual = sut.CanStart(dialog.Metadata);

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
        var dialog = new Dialog(new DialogMetadata("Id", canStart: true, "Name", "1.0.0"), // metadata says we can start
                                Enumerable.Empty<IDialogPart>(),
                                errorDialogPartMock.Object,
                                abortedDialogPartMock.Object,
                                completedDialogPartMock.Object,
                                Enumerable.Empty<IDialogPartGroup>());
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      dialog => new DialogContextFixture("Id", dialog.Metadata, errorDialogPartMock.Object, DialogState.InProgress)); // dialog state is already in progress
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);

        // Act
        var actual = sut.CanStart(dialog.Metadata);

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
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repository, conditionEvaluator);
        var dialog = DialogFixture.CreateBuilder().Build();
        var act = new Action(() => sut.Start(dialog.Metadata));

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
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var dialog = dialogMock.Object;
        var act = new Action(() => sut.Start(dialog.Metadata));

        // Act
        act.Should().ThrowExactly<InvalidOperationException>().WithMessage("Could not start dialog");
    }

    [Fact]
    public void Start_Uses_Result_From_RedirectPart()
    {
        // Arrange
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var errorDialogPart = new ErrorDialogPart("Something went horribly wrong!", "Error");
        var abortedPart = new AbortedDialogPart("Dialog has been aborted", "Abort");
        var completedPart = new CompletedDialogPart("Thank you for your input!", group2, "Completed", "Completed");
        var welcomePart = new MessageDialogPart("Welcome! I would like to answer a question", group1, "Welcome", "Welcome");
        var dialog2 = new Dialog
        (
            new DialogMetadata(
                "Dialog 2",
                true,
                "Dialog2",
                "1.0.0"),
            new IDialogPart[] { welcomePart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var redirectPart = new RedirectDialogPart(dialog2.Metadata, "Redirect");
        var dialog1 = new Dialog
        (
            new DialogMetadata(
                "Dialog 1",
                true,
                "Dialog1",
                "1.0.0"),
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
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);

        // Act
        var result = sut.Start(dialog1.Metadata);

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
        var errorDialogPart = new ErrorDialogPart("Something went horribly wrong!", "Error");
        var abortedPart = new AbortedDialogPart("Dialog has been aborted", "Abort");
        var completedPart = new CompletedDialogPart("Thank you for your input!", group2, "Completed", "Completed");
        var welcomePart = new MessageDialogPart("Welcome! I would like to answer a question", group1, "Welcome", "Welcome");
        var navigationPart = new NavigationDialogPart("Navigate", welcomePart.Id);
        var dialog = new Dialog
        (
            new DialogMetadata(
                "Test dialog",
                true,
                "Test",
                "1.0.0"),
            new IDialogPart[] { navigationPart, welcomePart },
            errorDialogPart,
            abortedPart,
            completedPart,
            Enumerable.Empty<IDialogPartGroup>()
        );
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);

        // Act
        var result = sut.Start(dialog.Metadata);

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroup.Should().Be(welcomePart.Group);
        result.CurrentPart.Id.Should().Be(welcomePart.Id);
    }

    [Fact]
    public void Start_Throws_When_Context_Could_Not_Be_Created()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => throw new InvalidOperationException("Kaboom"));
        var repository = new TestDialogRepository();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repository, conditionEvaluator);
        var start = new Action(() => sut.Start(dialog.Metadata));

        // Act
        start.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    [Fact]
    public void Start_Throws_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var start = new Action(() => sut.Start(dialog.Metadata));

        // Act
        start.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog: Id [DialogFixture], Version [1.0.0]");
    }

    [Fact]
    public void Start_Throws_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var start = new Action(() => sut.Start(dialog.Metadata));

        // Act
        start.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    [Fact]
    public void Start_Returns_ErrorDialogPart_When_First_DialogPart_Could_Not_Be_Determined()
    {
        // Arrange
        var dialog = DialogFixture.CreateHowDoYouFeelBuilder(false).Build();
        var factory = new DialogContextFactory();
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);

        // Act
        var result = sut.Start(dialog.Metadata);

        // Assert
        result.CurrentGroup.Should().BeNull();
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        //var errorDialogPart = (IErrorDialogPart)result.CurrentPart;
        //errorDialogPart.Exception.Should().NotBeNull();
        //errorDialogPart.Exception!.Message.Should().Be("Could not determine next part. Dialog does not have any parts.");
    }

    [Fact]
    public void Start_Returns_First_DialogPart_When_It_Could_Be_Determined()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var sut = CreateSut();

        // Act
        var result = sut.Start(dialog.Metadata);

        // Assert
        result.CurrentGroup.Should().Be(dialog.Parts.OfType<IGroupedDialogPart>().First().Group);
        result.CurrentPart.Id.Should().Be(dialog.Parts.First().Id);
    }

    [Fact]
    public void Start_Returns_ErrorDialogPart_When_DecisionPart_Returns_ErrorDialogPart()
    {
        // Arrange
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var errorDialogPart = new ErrorDialogPart("Something went horribly wrong!", "Error");
        var abortedPart = new AbortedDialogPart("Dialog has been aborted", "Abort");
        var completedPart = new CompletedDialogPart("Thank you for your input!", group2, "Completed", "Completed");
        var decisionPart = new DecisionDialogPartBuilder().WithId("Decision").WithDefaultNextPartId(errorDialogPart.Id).Build();
        var dialog = new Dialog
        (
            new DialogMetadata(
                "Test dialog",
                true,
                "Test",
                "1.0.0"),
            new IDialogPart[] { decisionPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);

        // Act
        var result = sut.Start(dialog.Metadata);

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
        var errorDialogPart = new ErrorDialogPart("Something went horribly wrong!", "Error");
        var abortedPart = new AbortedDialogPart("Dialog has been aborted", "Abort");
        var completedPart = new CompletedDialogPart("Thank you for your input!", group2, "Completed", "Completed");
        var decisionPart = new DecisionDialogPartBuilder().WithId("Decision").WithDefaultNextPartId(abortedPart.Id).Build();
        var dialog = new Dialog
        (
            new DialogMetadata(
                "Test dialog",
                true,
                "Test",
                "1.0.0"),
            new IDialogPart[] { decisionPart },
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
        );
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);

        // Act
        var result = sut.Start(dialog.Metadata);

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
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var completedPart = dialog.CompletedPart;
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(context, completedPart);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Requested_Part_Is_Not_Part_Of_Current_Dialog()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var errorPart = dialog.ErrorPart;
        var completedPart = dialog.CompletedPart;
        var context = new DialogContextFixture(Id, dialog.Metadata, errorPart, DialogState.InProgress);
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(context, completedPart);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Requested_Part_Is_After_Current_Part()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, messagePart, DialogState.InProgress);
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(context, questionPart);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_Part_Is_Current_Part()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(context, questionPart);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_Part_Is_Before_Current_Part()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        IDialogContext context = new DialogContextFixture(Id, dialog.Metadata, messagePart, DialogState.InProgress);
        context = context.AddDialogPartResults(dialog, new[] { new DialogPartResult(messagePart.Id, string.Empty, new EmptyDialogPartResultValue()) });
        context = context.Continue(dialog, questionPart);
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(context, messagePart);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void NavigateTo_Returns_Error_When_CanNavigateTo_Is_False()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, messagePart, DialogState.InProgress);
        var sut = CreateSut();

        // Act
        var result = sut.NavigateTo(context, questionPart);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
    }

    [Fact]
    public void NavigateTo_Navigates_To_Requested_Part_When_CanNavigate_Is_True()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        IDialogContext context = new DialogContextFixture(Id, dialog.Metadata, messagePart, DialogState.InProgress);
        context = context.AddDialogPartResults(dialog, new[] { new DialogPartResult(messagePart.Id, string.Empty, new EmptyDialogPartResultValue()) });
        context = context.Continue(dialog, questionPart);
        var sut = CreateSut();

        // Act
        var result = sut.NavigateTo(context, messagePart);

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroup.Should().Be(messagePart.Group);
        result.CurrentPart.Should().BeAssignableTo<IMessageDialogPart>();
        result.CurrentPart.Id.Should().Be(messagePart.Id);
    }

    [Fact]
    public void NavigateTo_Throws_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var navigate = new Action(() => sut.NavigateTo(factory.Create(dialog), dialog.Parts.First()));

        // Act
        navigate.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog: Id [DialogFixture], Version [1.0.0]");
    }

    [Fact]
    public void NavigateTo_Throws_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var navigate = new Action(() => sut.NavigateTo(factory.Create(dialog), dialog.Parts.First()));

        // Act
        navigate.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    [Theory]
    [InlineData(DialogState.Aborted)]
    [InlineData(DialogState.Completed)]
    [InlineData(DialogState.ErrorOccured)]
    [InlineData(DialogState.Initial)]
    public void CanResetCurrentState_Returns_False_When_CurrentState_Is(DialogState currentState)
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, currentState);
        var sut = CreateSut();

        // Act
        var result = sut.CanResetCurrentState(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanResetCurrentState_Returns_False_When_Current_DialogPart_Is_Not_QuestionDialogPart()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = new DialogContextFixture(Id, dialog.Metadata, dialog.CompletedPart, DialogState.InProgress); // note that this actually invalid state, but we currently can't prevent it on the interface
        var sut = CreateSut();

        // Act
        var result = sut.CanResetCurrentState(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanResetCurrentState_Returns_True_When_CurrentState_Is_InProgress_And_Current_DialogPart_Is_QuestionDialogPart()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        var sut = CreateSut();

        // Act
        var result = sut.CanResetCurrentState(context);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ResetCurrentState_Resets_Answers_From_Current_Question_When_All_Is_Good()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        context = new DialogContextFixture(context, new[]
        {
            new DialogPartResult(questionPart.Id, "Terrible", new EmptyDialogPartResultValue()),
            new DialogPartResult("Other part", "Other value", new EmptyDialogPartResultValue())
        });
        var sut = CreateSut();

        // Act
        var result = sut.ResetCurrentState(context);

        // Assert
        var dialogPartResults = result.Results;
        dialogPartResults.Should().ContainSingle();
        dialogPartResults.Single().DialogPartId.Should().Be("Other part");
    }

    [Fact]
    public void ResetCurrentState_Returns_ErrorDialogPart_When_CanResetCurrentState_Is_False()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = new DialogContextFixture(Id, dialog.Metadata, questionPart, DialogState.Aborted);
        var sut = CreateSut();

        // Act
        var result = sut.ResetCurrentState(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroup.Should().BeNull();
        result.CurrentPart.Should().BeAssignableTo<IErrorDialogPart>();
        result.CurrentPart.Id.Should().Be(dialog.ErrorPart.Id);
    }

    [Fact]
    public void ResetCurrentState_Throws_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var resetCurrentState = new Action(() => sut.ResetCurrentState(factory.Create(dialog)));

        // Act
        resetCurrentState.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog: Id [DialogFixture], Version [1.0.0]");
    }

    [Fact]
    public void ResetCurrentState_Throws_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == dialog.Metadata.Id,
                                                      _ => new DialogContextFixture(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator);
        var resetCurrentState = new Action(() => sut.ResetCurrentState(factory.Create(dialog)));

        // Act
        resetCurrentState.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    private static DialogService CreateSut()
    {
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        return new DialogService(factory, repository, conditionEvaluator);
    }

    private static DialogService CreateSutForTwoDialogsWithRedirect(out IDialog dialog1)
    {
        var group1 = new DialogPartGroupBuilder()
            .WithId("Part1")
            .WithTitle("Give information")
            .WithNumber(1);
        var group2 = new DialogPartGroupBuilder()
            .WithId("Part2")
            .WithTitle("Completed")
            .WithNumber(2);
        var errorDialogPart = new ErrorDialogPartBuilder()
            .WithErrorMessage("Something went horribly wrong!")
            .WithId("Error");
        var abortedPart = new AbortedDialogPartBuilder()
            .WithMessage("Dialog has been aborted")
            .WithId("Abort");
        var completedPart = new CompletedDialogPartBuilder()
            .WithMessage("Thank you for your input!")
            .WithGroup(group2)
            .WithHeading("Thank you")
            .WithId("Completed");
        var welcomePart = new MessageDialogPartBuilder()
            .WithMessage("Welcome! I would like to answer a question")
            .WithGroup(group1)
            .WithHeading("Welcome")
            .WithId("Welcome");
        var dialog2 = new DialogBuilder()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 2")
                .WithId("Dialog2")
                .WithVersion("1.0.0"))
            .AddParts(new DialogPartBuilder(welcomePart))
            .WithErrorPart(errorDialogPart)
            .WithAbortedPart(abortedPart)
            .WithCompletedPart(completedPart)
            .AddPartGroups(group1, group2)
            .Build();
        var redirectPart = new RedirectDialogPartBuilder()
            .WithRedirectDialogMetadata(new DialogMetadataBuilder(dialog2.Metadata))
            .WithId("Redirect");
        dialog1 = new DialogBuilder()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 1")
                .WithId("Dialog1")
                .WithVersion("1.0.0"))
            .AddParts(new DialogPartBuilder(welcomePart), new DialogPartBuilder(redirectPart))
            .WithErrorPart(errorDialogPart)
            .WithAbortedPart(abortedPart)
            .WithCompletedPart(completedPart)
            .AddPartGroups(group1)
            .Build();
        var id1 = dialog1.Metadata.Id;
        var metadata1 = dialog1.Metadata;
        var factory = new DialogContextFactoryFixture(d => d.Metadata.Id == id1,
                                                      _ => new DialogContextFixture(metadata1));
        var repositoryMock = new Mock<IDialogRepository>();
        var d1 = dialog1;
        var d2 = dialog2;
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns<IDialogIdentifier>(identifier =>
        {
            if (identifier.Id == "Dialog1") return d1;
            if (identifier.Id == "Dialog2") return d2;
            return null;
        });
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        return new DialogService(factory, repositoryMock.Object, conditionEvaluator);
    }
}
