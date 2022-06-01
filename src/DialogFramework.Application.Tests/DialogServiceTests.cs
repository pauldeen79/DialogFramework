namespace DialogFramework.Application.Tests;

public class DialogServiceTests
{
    private readonly Mock<ILogger> _loggerMock;

    public DialogServiceTests()
    {
        _loggerMock = new Mock<ILogger>();
    }

    private static string Id => Guid.NewGuid().ToString();

    [Theory]
    [InlineData(DialogState.Aborted)]
    [InlineData(DialogState.InProgress)]
    public void Abort_Returns_ErrorDialogPart_When_Already_Aborted(DialogState currentState)
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var abortedPart = dialog.AbortedPart;
        var context = DialogContextFixture.Create(Id, dialog.Metadata, abortedPart, currentState);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroupId.Should().BeNull();
        _loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Abort failed" && @type.Name == "FormattedLogValues"),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Fact]
    public void Abort_Returns_ErrorDialogPart_When_Already_Completed()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = DialogContextFixture.Create(Id, dialog.Metadata, dialog.CompletedPart, DialogState.Completed);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroupId.Should().BeNull();
        _loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Abort failed" && @type.Name == "FormattedLogValues"),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Fact]
    public void Abort_Returns_ErrorDialogPart_When_Dialog_Is_In_ErrorState()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var context = DialogContextFixture.Create(Id, dialog.Metadata, dialog.ErrorPart, DialogState.ErrorOccured);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroupId.Should().BeNull();
        _loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Abort failed" && @type.Name == "FormattedLogValues"),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Fact]
    public void Abort_Returns_AbortDialogPart_Dialog_When_Dialog_Is_InProgress()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var abortedPart = dialog.AbortedPart;
        var context = DialogContextFixture.Create(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.Aborted);
        result.CurrentPartId.Should().BeEquivalentTo(abortedPart.Id);
        result.CurrentGroupId.Should().BeNull();
    }

    [Fact]
    public void Abort_Throws_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var abort = new Action(() => sut.Abort(factory.Create(dialog)));

        // Act
        abort.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog: Id [DialogFixture], Version [1.0.0]");
    }

    [Fact]
    public void Abort_Throws_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
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
        var context = DialogContextFixture.Create(Id, dialog.Metadata, questionPart, currentState);
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
        var context = DialogContextFixture.Create(Id, dialog.Metadata, abortedPart, DialogState.InProgress);
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
        var context = DialogContextFixture.Create(Id, dialog.Metadata, currentPart, currentState);
        var sut = CreateSut();

        // Act
        var result = sut.Continue(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroupId.Should().BeNull();
        _loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Continue failed" && @type.Name == "FormattedLogValues"),
            It.Is<InvalidOperationException>(ex => ex.Message == "Can only continue when the dialog is in progress"),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Fact]
    public void Continue_Returns_Next_DialogPart_When_Current_State_Is_Question_And_Answer_Is_Valid()
    {
        // Arrange
        var dialog = DialogFixture.CreateHowDoYouFeelBuilder().Build();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = DialogContextFixture.Create(Id, dialog.Metadata, currentPart, currentState);
        var sut = CreateSut();
        var dialogPartResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Great"))
            .Build();

        // Act
        var result = sut.Continue(context, new[] { dialogPartResult });

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentPartId.Value.Should().Be("Completed");
        result.CurrentGroupId.Should().BeEquivalentTo(dialog.CompletedPart.Group.Id);
    }

    [Fact]
    public void Continue_Returns_Same_DialogPart_When_Current_State_Is_Question_And_Answer_Is_Not_Valid()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = DialogContextFixture.Create(Id, dialog.Metadata, currentPart, currentState);
        var sut = CreateSut();
        var dialogPartResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Unknown result"))
            .Build();

        // Act
        var result = sut.Continue(context, new[] { dialogPartResult });

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroupId.Should().BeEquivalentTo(currentPart.GetGroupId());
        result.ValidationErrors.Should().ContainSingle();
        result.ValidationErrors.Single().ErrorMessage.Should().Be("Unknown Result Id: [DialogPartResultIdentifier { Value = Unknown result }]");
    }

    [Fact]
    public void Continue_Returns_Same_DialogPart_On_Answers_From_Wrong_Question()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.InProgress;
        var context = DialogContextFixture.Create(Id, dialog.Metadata, currentPart, currentState);
        var sut = CreateSut();
        var wrongQuestionMock = new Mock<IQuestionDialogPart>();
        wrongQuestionMock.SetupGet(x => x.Results).Returns(new ReadOnlyValueCollection<IDialogPartResultDefinition>());
        wrongQuestionMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifierBuilder().Build());
        var dialogPartResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(wrongQuestionMock.Object.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Unknown answer"))
            .Build();

        // Act
        var result = sut.Continue(context, new[] { dialogPartResult });

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.ValidationErrors.Should().ContainSingle();
        result.ValidationErrors.Single().ErrorMessage.Should().Be("Provided answer from wrong question");
    }

    [Fact]
    public void Continue_Uses_Result_From_DecisionPart_When_DecisionPart_Returns_No_Error()
    {
        // Arrange
        var dialog = DialogFixture.CreateHowDoYouFeelBuilder().Build();
        var currentPart = dialog.Parts.OfType<IQuestionDialogPart>().First();
        var currentState = DialogState.Initial;
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(Id, dialog.Metadata, currentPart, currentState));
        var repository = new TestDialogRepository();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repository, conditionEvaluator, _loggerMock.Object);
        var context = sut.Start(dialog.Metadata); // start the dialog, this will get the welcome message
        context = sut.Continue(context); // skip the welcome message
        var dialogPartResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Terrible"))
            .Build();

        // Act
        var result = sut.Continue(context, new[] { dialogPartResult }); // answer the question with 'Terrible', this will trigger a second message

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentPartId.Value.Should().Be("Completed");
        result.CurrentGroupId.Should().BeEquivalentTo(dialog.CompletedPart.Group.Id);
    }

    [Fact]
    public void Continue_Does_Not_Work_On_RedirectPart()
    {
        // Arrange
        var dialog2 = DialogFixture.CreateBuilder();
        dialog2.Metadata.Id = "Dialog2";
        var redirectPart = new RedirectDialogPartBuilder()
            .WithRedirectDialogMetadata(dialog2.Metadata)
            .WithId(new DialogPartIdentifierBuilder().WithValue("Redirect"));
        var dialog1 = DialogFixture.CreateBuilder();
        dialog1.Parts.Clear();
        dialog1.Parts.Add(redirectPart);
        dialog1.Metadata.Id = "Dialog1";
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog1.Metadata.Id) || Equals(d.Metadata.Id, dialog2.Metadata.Id),
                                                      d => Equals(d.Metadata.Id, dialog1.Metadata.Id)
                                                          ? DialogContextFixture.Create(dialog1.Metadata.Build())
                                                          : DialogContextFixture.Create(dialog2.Metadata.Build()));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns<IDialogIdentifier>(identifier =>
        {
            if (Equals(identifier.Id, dialog1.Metadata.Id) && Equals(identifier.Version, dialog1.Metadata.Version)) return dialog1.Build();
            if (Equals(identifier.Id, dialog2.Metadata.Id) && Equals(identifier.Version, dialog2.Metadata.Version)) return dialog2.Build();
            return null;
        });
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var context = sut.Start(dialog1.Metadata.Build()); // this will trigger the message on dialog 1

        // Act
        var result = sut.Continue(context); // this will trigger the redirect to dialog 2

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentPartId.Value.Should().Be("Error");
        result.Errors.Select(x => x.Message).Should().BeEquivalentTo(new[] { "Continue failed" });
    }

    [Fact]
    public void Continue_Uses_Result_From_NavigationPart()
    {
        // Arrange
        var welcomePart = new MessageDialogPartBuilder()
            .WithMessage("Welcome! I would like to answer a question")
            .WithGroup(DialogPartGroupFixture.CreateBuilder())
            .WithHeading("Welcome")
            .WithId(new DialogPartIdentifierBuilder().WithValue("Welcome"));
        var navigatedPart = new MessageDialogPartBuilder()
            .WithMessage("This shows that navigation works")
            .WithGroup(DialogPartGroupFixture.CreateBuilder())
            .WithHeading("Navigated")
            .WithId(new DialogPartIdentifierBuilder().WithValue("Navigated"));
        var navigationPart = new NavigationDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Navigate"))
            .WithNavigateToId(navigatedPart.Id);
        var dialog = DialogFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Test dialog")
                .WithId("Test")
                .WithVersion("1.0.0"))
            .AddParts
            (
                welcomePart,
                navigationPart,
                navigatedPart
            )
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder())
            .Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var context = sut.Start(dialog.Metadata); // this will trigger the message

        // Act
        var result = sut.Continue(context); // this will trigger the navigation

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroupId.Should().BeEquivalentTo(navigatedPart.Group.Build().Id);
        result.CurrentPartId.Should().BeEquivalentTo(navigatedPart.Id);
    }

    [Theory]
    [InlineData(DialogState.Aborted)]
    [InlineData(DialogState.Completed)]
    [InlineData(DialogState.ErrorOccured)]
    public void Continue_Returns_ErrorDialogPart_When_State_Is_Wrong(DialogState currentState)
    {
        // Arrange
        var sut = CreateSutForTwoDialogsWithRedirect(out var dialog1);
        var context = DialogContextFixture.Create(Id, dialog1.Metadata, dialog1.Parts.First(), currentState);

        // Act
        var result = sut.Continue(context); // this will trigger the redirect to dialog 2

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroupId.Should().BeNull();
        _loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Continue failed" && @type.Name == "FormattedLogValues"),
            It.Is<InvalidOperationException>(ex => ex.Message == "Can only continue when the dialog is in progress"),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Fact]
    public void Continue_Returns_CompletedDialogPart_When_There_Is_No_Next_DialogPart()
    {
        // Arrange
        var welcomePart = new MessageDialogPartBuilder()
            .WithMessage("Welcome! I would like to answer a question")
            .WithGroup(DialogPartGroupFixture.CreateBuilder())
            .WithHeading("Welcome")
            .WithId(new DialogPartIdentifierBuilder().WithValue("Welcome"));
        var dialog = DialogFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Test dialog")
                .WithId("Test")
                .WithVersion("1.0.0"))
            .AddParts(welcomePart)
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder())
            .Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var context = sut.Start(dialog.Metadata); // this will trigger the message

        // Act
        var result = sut.Continue(context); // this will trigger the completion

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentPartId.Value.Should().Be("Completed");
    }

    [Fact]
    public void Continue_Throws_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var continuation = new Action(() => sut.Continue(factory.Create(dialog)));

        // Act
        continuation.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog: Id [DialogFixture], Version [1.0.0]");
    }

    [Fact]
    public void Continue_Throws_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
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
        var context = DialogContextFixture.Create(Id, dialog.Metadata, questionPart, currentState);
        var sut = CreateSut();

        // Act
        var result = sut.CanContinue(context, Enumerable.Empty<IDialogPartResult>());

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public void CanStart_Returns_Correct_Value_Based_On_Dialog_CanStart(bool dialogCanStart, bool expectedResult)
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder().WithFriendlyName("Name").WithCanStart(dialogCanStart).WithId("Id").WithVersion("1.0.0"))
            .AddParts(new MessageDialogPartBuilder().WithId(new DialogPartIdentifierBuilder()).WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder())))
            .Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      dialog => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);

        // Act
        var actual = sut.CanStart(dialog.Metadata);

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Fact]
    public void CanStart_Returns_False_When_CurrentState_Is_InProgress()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder().WithFriendlyName("Name").WithCanStart(true).WithId("Id").WithVersion("1.0.0"))
            .AddParts(new MessageDialogPartBuilder().WithId(new DialogPartIdentifierBuilder()).WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder())))
            .Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      dialog => DialogContextFixture.Create("Id", dialog.Metadata, dialog.ErrorPart, DialogState.InProgress)); // dialog state is already in progress
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);

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
        var sut = new DialogService(factory, repository, conditionEvaluator, _loggerMock.Object);
        var dialog = DialogFixture.CreateBuilder().Build();
        var act = new Action(() => sut.Start(dialog.Metadata));

        // Act
        act.Should().ThrowExactly<InvalidOperationException>().WithMessage("Could not create context");
    }

    [Fact]
    public void Start_Returns_Error_When_CanStart_Is_False()
    {
        // Arrange
        var dialogMetadataMock = new Mock<IDialogMetadata>();
        dialogMetadataMock.SetupGet(x => x.CanStart).Returns(false);
        dialogMetadataMock.SetupGet(x => x.Id).Returns("Empty");
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifierBuilder().Build());
        var errorPartMock = new Mock<IErrorDialogPart>();
        errorPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifierBuilder().Build());
        var dialogMock = new Mock<IDialog>();
        dialogMock.SetupGet(x => x.Metadata).Returns(dialogMetadataMock.Object);
        dialogMock.SetupGet(x => x.ErrorPart).Returns(errorPartMock.Object);
        dialogMock.Setup(x => x.GetFirstPart(It.IsAny<IDialogContext>(), It.IsAny<IConditionEvaluator>())).Returns(dialogPartMock.Object);
        var factory = new DialogContextFactoryFixture(_ => true,
                                                      _ => DialogContextFixture.Create("Id", dialogMock.Object.Metadata, dialogPartMock.Object, DialogState.Initial));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialogMock.Object);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var dialog = dialogMock.Object;

        // Act
        var result = sut.Start(dialog.Metadata);

        // Act
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Errors.Select(x => x.Message).Should().BeEquivalentTo(new[] { "Start failed" });
    }

    [Fact]
    public void Start_Can_Return_RedirectPart()
    {
        // Arrange
        var welcomePart = new MessageDialogPartBuilder()
            .WithMessage("Welcome! I would like to answer a question")
            .WithGroup(DialogPartGroupFixture.CreateBuilder())
            .WithId(new DialogPartIdentifierBuilder().WithValue("Welcome"))
            .WithHeading("Welcome");
        var dialog2 = DialogFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 2")
                .WithId("Dialog2")
                .WithVersion("1.0.0"))
            .AddParts(welcomePart)
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder()).Build();
        var redirectPart = new RedirectDialogPartBuilder()
            .WithRedirectDialogMetadata(new DialogMetadataBuilder(dialog2.Metadata))
            .WithId(new DialogPartIdentifierBuilder().WithValue("Redirect"));
        var dialog1 = DialogFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 1")
                .WithId("Dialog1")
                .WithVersion("1.0.0"))
            .AddParts(redirectPart)
            .Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog1.Metadata.Id) || Equals(d.Metadata.Id, dialog2.Metadata.Id),
                                                      dialog => Equals(dialog.Metadata.Id, dialog1.Metadata.Id)
                                                          ? DialogContextFixture.Create(dialog1.Metadata)
                                                          : DialogContextFixture.Create(dialog2.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns<IDialogIdentifier>(identifier =>
        {
            if (Equals(identifier.Id, dialog1.Metadata.Id) && Equals(identifier.Version, dialog1.Metadata.Version)) return dialog1;
            if (Equals(identifier.Id, dialog2.Metadata.Id) && Equals(identifier.Version, dialog2.Metadata.Version)) return dialog2;
            return null;
        });
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);

        // Act
        var result = sut.Start(dialog1.Metadata);

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentDialogIdentifier.Id.Should().BeEquivalentTo(dialog1.Metadata.Id);
        result.CurrentGroupId.Should().BeNull();
        result.CurrentPartId.Value.Should().Be("Redirect");
    }

    [Fact]
    public void Start_Uses_Result_From_NavigationPart()
    {
        // Arrange
        var welcomePart = new MessageDialogPartBuilder()
            .WithMessage("Welcome! I would like to answer a question")
            .WithGroup(DialogPartGroupFixture.CreateBuilder())
            .WithId(new DialogPartIdentifierBuilder().WithValue("Welcome"))
            .WithHeading("Welcome");
        var navigationPart = new NavigationDialogPartBuilder().WithId(new DialogPartIdentifierBuilder().WithValue("Navigate")).WithNavigateToId(welcomePart.Id);
        var dialog = DialogFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Test dialog")
                .WithId("Test")
                .WithVersion("1.0.0"))
            .AddParts(navigationPart, welcomePart)
            .Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);

        // Act
        var result = sut.Start(dialog.Metadata);

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroupId.Should().BeEquivalentTo(welcomePart.Group.Id);
        result.CurrentPartId.Should().BeEquivalentTo(welcomePart.Id);
    }

    [Fact]
    public void Start_Throws_When_Context_Could_Not_Be_Created()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => throw new InvalidOperationException("Kaboom"));
        var repository = new TestDialogRepository();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repository, conditionEvaluator, _loggerMock.Object);
        var start = new Action(() => sut.Start(dialog.Metadata));

        // Act
        start.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    [Fact]
    public void Start_Throws_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var start = new Action(() => sut.Start(dialog.Metadata));

        // Act
        start.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog: Id [DialogFixture], Version [1.0.0]");
    }

    [Fact]
    public void Start_Throws_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
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
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);

        // Act
        var result = sut.Start(dialog.Metadata);

        // Assert
        result.CurrentGroupId.Should().BeNull();
        _loggerMock.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "Start failed" && @type.Name == "FormattedLogValues"),
            It.Is<InvalidOperationException>(ex => ex.Message == "Could not determine next part. Dialog does not have any parts."),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
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
        result.CurrentGroupId.Should().BeEquivalentTo(dialog.Parts.OfType<IGroupedDialogPart>().First().Group.Id);
        result.CurrentPartId.Should().BeEquivalentTo(dialog.Parts.First().Id);
    }

    [Fact]
    public void Start_Returns_ErrorDialogPart_When_DecisionPart_Returns_ErrorDialogPart()
    {
        // Arrange
        var decisionPart = new DecisionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Decision"))
            .WithDefaultNextPartId(new DialogPartIdentifierBuilder().WithValue("Error"));
        var dialog = DialogFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Test dialog")
                .WithId("Test")
                .WithVersion("1.0.0"))
            .AddParts(decisionPart)
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder())
            .Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);

        // Act
        var result = sut.Start(dialog.Metadata);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroupId.Should().BeNull();
        result.CurrentPartId.Value.Should().Be("Error");
    }

    [Fact]
    public void Start_Returns_AbortDialogPart_When_DecisionPart_Returns_AbortDialogPart()
    {
        // Arrange
        var decisionPart = new DecisionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Decision"))
            .WithDefaultNextPartId(new DialogPartIdentifierBuilder().WithValue("Abort"));
        var dialog = DialogFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Test dialog")
                .WithId("Test")
                .WithVersion("1.0.0"))
            .AddParts(decisionPart)
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder())
            .Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns(dialog);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);

        // Act
        var result = sut.Start(dialog.Metadata);

        // Assert
        result.CurrentState.Should().Be(DialogState.Aborted);
        result.CurrentGroupId.Should().BeNull();
        result.CurrentPartId.Value.Should().Be("Abort");
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Parts_Does_Not_Contain_Current_Part()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var completedPart = dialog.CompletedPart;
        var context = DialogContextFixture.Create(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(context, completedPart.Id);

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
        var context = DialogContextFixture.Create(Id, dialog.Metadata, errorPart, DialogState.InProgress);
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(context, completedPart.Id);

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
        var context = DialogContextFixture.Create(Id, dialog.Metadata, messagePart, DialogState.InProgress);
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(context, questionPart.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_Part_Is_Current_Part()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = DialogContextFixture.Create(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(context, questionPart.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_Part_Is_Before_Current_Part()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        IDialogContext context = DialogContextFixture.Create(Id, dialog.Metadata, messagePart, DialogState.InProgress);
        var partResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(messagePart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(string.Empty))
            .Build();
        context = context.Chain(x => x.Continue(dialog, new[] { partResult }, conditionEvaluatorMock.Object, Enumerable.Empty<IDialogValidationResult>()));
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(context, messagePart.Id);

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
        var context = DialogContextFixture.Create(Id, dialog.Metadata, messagePart, DialogState.InProgress);
        var sut = CreateSut();

        // Act
        var result = sut.NavigateTo(context, questionPart.Id);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
    }

    [Fact]
    public void NavigateTo_Navigates_To_Requested_Part_When_CanNavigate_Is_True()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var messagePart = dialog.Parts.OfType<IMessageDialogPart>().First();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        IDialogContext context = DialogContextFixture.Create(Id, dialog.Metadata, messagePart, DialogState.InProgress);
        var partResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(messagePart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(string.Empty))
            .Build();
        context = context.Chain(x => x.Continue(dialog, new[] { partResult }, conditionEvaluatorMock.Object, Enumerable.Empty<IDialogValidationResult>()));
        var sut = CreateSut();

        // Act
        var result = sut.NavigateTo(context, messagePart.Id);

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroupId.Should().BeEquivalentTo(messagePart.Group.Id);
        result.CurrentPartId.Should().BeEquivalentTo(messagePart.Id);
    }

    [Fact]
    public void NavigateTo_Throws_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var navigate = new Action(() => sut.NavigateTo(factory.Create(dialog), dialog.Parts.First().Id));

        // Act
        navigate.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog: Id [DialogFixture], Version [1.0.0]");
    }

    [Fact]
    public void NavigateTo_Throws_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var navigate = new Action(() => sut.NavigateTo(factory.Create(dialog), dialog.Parts.First().Id));

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
        var context = DialogContextFixture.Create(Id, dialog.Metadata, questionPart, currentState);
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
        var context = DialogContextFixture.Create(Id, dialog.Metadata, dialog.CompletedPart, DialogState.InProgress); // note that this actually invalid state, but we currently can't prevent it on the interface
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
        var context = DialogContextFixture.Create(Id, dialog.Metadata, questionPart, DialogState.InProgress);
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
        var context = DialogContextFixture.Create(Id, dialog.Metadata, questionPart, DialogState.InProgress);
        context = DialogContextFixture.Create(context, new[]
        {
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(questionPart.Id))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Terrible"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("Other part"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Other value"))
                .Build()
        });
        var sut = CreateSut();

        // Act
        var result = sut.ResetCurrentState(context);

        // Assert
        var dialogPartResults = result.Results;
        dialogPartResults.Should().ContainSingle();
        dialogPartResults.Single().DialogPartId.Value.Should().Be("Other part");
    }

    [Fact]
    public void ResetCurrentState_Returns_ErrorDialogPart_When_CanResetCurrentState_Is_False()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var questionPart = dialog.Parts.OfType<IQuestionDialogPart>().Single();
        var context = DialogContextFixture.Create(Id, dialog.Metadata, questionPart, DialogState.Aborted);
        var sut = CreateSut();

        // Act
        var result = sut.ResetCurrentState(context);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroupId.Should().BeNull();
        result.CurrentPartId.Should().BeEquivalentTo(dialog.ErrorPart.Id);
    }

    [Fact]
    public void ResetCurrentState_Throws_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var resetCurrentState = new Action(() => sut.ResetCurrentState(factory.Create(dialog)));

        // Act
        resetCurrentState.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog: Id [DialogFixture], Version [1.0.0]");
    }

    [Fact]
    public void ResetCurrentState_Throws_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialog = DialogFixture.CreateBuilder().Build();
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, dialog.Metadata.Id),
                                                      _ => DialogContextFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogRepository>();
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var resetCurrentState = new Action(() => sut.ResetCurrentState(factory.Create(dialog)));

        // Act
        resetCurrentState.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    private DialogService CreateSut()
    {
        var factory = new DialogContextFactory();
        var repository = new TestDialogRepository();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        return new DialogService(factory, repository, conditionEvaluator, _loggerMock.Object);
    }

    private DialogService CreateSutForTwoDialogsWithRedirect(out IDialog dialog1)
    {
        var welcomePart = new MessageDialogPartBuilder()
            .WithMessage("Welcome! I would like to answer a question")
            .WithGroup(DialogPartGroupFixture.CreateBuilder())
            .WithHeading("Welcome")
            .WithId(new DialogPartIdentifierBuilder().WithValue("Welcome"));
        var dialog2 = DialogFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 2")
                .WithId("Dialog2")
                .WithVersion("1.0.0"))
            .AddParts(welcomePart)
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder())
            .Build();
        var redirectPart = new RedirectDialogPartBuilder()
            .WithRedirectDialogMetadata(new DialogMetadataBuilder(dialog2.Metadata))
            .WithId(new DialogPartIdentifierBuilder().WithValue("Redirect"));
        dialog1 = DialogFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 1")
                .WithId("Dialog1")
                .WithVersion("1.0.0"))
            .AddParts(welcomePart, redirectPart)
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder())
            .Build();
        var id1 = dialog1.Metadata.Id;
        var metadata1 = dialog1.Metadata;
        var factory = new DialogContextFactoryFixture(d => Equals(d.Metadata.Id, id1),
                                                      _ => DialogContextFixture.Create(metadata1));
        var repositoryMock = new Mock<IDialogRepository>();
        var d1 = dialog1;
        var d2 = dialog2;
        repositoryMock.Setup(x => x.GetDialog(It.IsAny<IDialogIdentifier>())).Returns<IDialogIdentifier>(identifier =>
        {
            if (Equals(identifier.Id, "Dialog1")) return d1;
            if (Equals(identifier.Id, "Dialog2")) return d2;
            return null;
        });
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        return new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
    }
}
