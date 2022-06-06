namespace DialogFramework.Application.Tests;

public class DialogServiceTests
{
    private readonly Mock<ILogger> _loggerMock;

    public DialogServiceTests()
    {
        _loggerMock = new Mock<ILogger>();
    }

    private static string Id => Guid.NewGuid().ToString();

    [Fact]
    public void Abort_Returns_ErrorDialogPart_When_Already_Aborted()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var abortedPart = dialogDefinition.AbortedPart;
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, abortedPart);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(dialog);

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
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.CompletedPart);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(dialog);

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
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.ErrorPart);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(dialog);

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
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var abortedPart = dialogDefinition.AbortedPart;
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(dialog);

        // Assert
        result.CurrentState.Should().Be(DialogState.Aborted);
        result.CurrentPartId.Should().BeEquivalentTo(abortedPart.Id);
        result.CurrentGroupId.Should().BeNull();
    }

    [Fact]
    public void Abort_Throws_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var abort = new Action(() => sut.Abort(factory.Create(dialogDefinition)));

        // Act
        abort.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]");
    }

    [Fact]
    public void Abort_Throws_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var abort = new Action(() => sut.Abort(factory.Create(dialogDefinition)));

        // Act
        abort.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    [Fact]
    public void CanAbort_Returns_Correct_Result_Based_On_Current_State()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        var sut = CreateSut();

        // Act
        var result = sut.CanAbort(dialog);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanAbort_Returns_False_When_CurrentPart_Is_AbortedPart()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var abortedPart = dialogDefinition.AbortedPart;
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, abortedPart);
        var sut = CreateSut();

        // Act
        var result = sut.CanAbort(dialog);

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
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        IDialogPart currentPart = currentState switch
        {
            DialogState.Aborted => dialogDefinition.AbortedPart,
            DialogState.Completed => dialogDefinition.CompletedPart,
            DialogState.ErrorOccured => dialogDefinition.ErrorPart,
            _ => throw new NotImplementedException()
        };
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, currentPart);
        var sut = CreateSut();

        // Act
        var result = sut.Continue(dialog);

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
        var dialogDefinition = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var currentPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().First();
        var definition = DialogFixture.Create(Id, dialogDefinition.Metadata, currentPart);
        var sut = CreateSut();
        var dialogPartResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Great"))
            .Build();

        // Act
        var result = sut.Continue(definition, new[] { dialogPartResult });

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentPartId.Value.Should().Be("Completed");
        result.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.CompletedPart.Group.Id);
    }

    [Fact]
    public void Continue_Returns_Same_DialogPart_When_Current_State_Is_Question_And_Answer_Is_Not_Valid()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var currentPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().First();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, currentPart);
        var sut = CreateSut();
        var dialogPartResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Unknown result"))
            .Build();

        // Act
        var result = sut.Continue(dialog, new[] { dialogPartResult });

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
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var currentPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().First();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, currentPart);
        var sut = CreateSut();
        var wrongQuestionMock = new Mock<IQuestionDialogPart>();
        wrongQuestionMock.SetupGet(x => x.Results).Returns(new ReadOnlyValueCollection<IDialogPartResultDefinition>());
        wrongQuestionMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifierBuilder().Build());
        var dialogPartResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(wrongQuestionMock.Object.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Unknown answer"))
            .Build();

        // Act
        var result = sut.Continue(dialog, new[] { dialogPartResult });

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.ValidationErrors.Should().ContainSingle();
        result.ValidationErrors.Single().ErrorMessage.Should().Be("Provided answer from wrong question");
    }

    [Fact]
    public void Continue_Uses_Result_From_DecisionPart_When_DecisionPart_Returns_No_Error()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var currentPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().First();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(Id, dialogDefinition.Metadata, currentPart));
        var repository = new TestDialogDefinitionRepository();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repository, conditionEvaluator, _loggerMock.Object);
        var dialog = sut.Continue(factory.Create(dialogDefinition)); // skip the welcome message
        var dialogPartResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Terrible"))
            .Build();

        // Act
        var result = sut.Continue(dialog, new[] { dialogPartResult }); // answer the question with 'Terrible', this will trigger a second message

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentPartId.Value.Should().Be("Completed");
        result.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.CompletedPart.Group.Id);
    }

    [Fact]
    public void Continue_Does_Not_Work_On_RedirectPart()
    {
        // Arrange
        var dialogDefinition2 = DialogDefinitionFixture.CreateBuilder();
        dialogDefinition2.Metadata.Id = "Dialog2";
        var redirectPart = new RedirectDialogPartBuilder()
            .WithRedirectDialogMetadata(dialogDefinition2.Metadata)
            .WithId(new DialogPartIdentifierBuilder().WithValue("Redirect"));
        var dialogDefinition1 = DialogDefinitionFixture.CreateBuilder();
        dialogDefinition1.Parts.Clear();
        dialogDefinition1.Parts.Add(redirectPart);
        dialogDefinition1.Metadata.Id = "Dialog1";
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition1.Metadata.Id) || Equals(d.Metadata.Id, dialogDefinition2.Metadata.Id),
                                               d => Equals(d.Metadata.Id, dialogDefinition1.Metadata.Id)
                                                   ? DialogFixture.Create(dialogDefinition1.Metadata.Build())
                                                   : DialogFixture.Create(dialogDefinition2.Metadata.Build()));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns<IDialogDefinitionIdentifier>(identifier =>
        {
            if (Equals(identifier.Id, dialogDefinition1.Metadata.Id) && Equals(identifier.Version, dialogDefinition1.Metadata.Version)) return dialogDefinition1.Build();
            if (Equals(identifier.Id, dialogDefinition2.Metadata.Id) && Equals(identifier.Version, dialogDefinition2.Metadata.Version)) return dialogDefinition2.Build();
            return null;
        });
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var dialog = sut.Start(dialogDefinition1.Metadata.Build()); // this will trigger the message on dialog 1

        // Act
        var result = sut.Continue(dialog); // this will trigger the redirect to dialog 2

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
        var dialogDefinition = DialogDefinitionFixture.CreateBuilderBase()
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
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinition);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var dialog = sut.Start(dialogDefinition.Metadata); // this will trigger the message

        // Act
        var result = sut.Continue(dialog); // this will trigger the navigation

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
        IDialogPart currentPart = currentState switch
        {
            DialogState.Aborted => dialog1.AbortedPart,
            DialogState.Completed => dialog1.CompletedPart,
            DialogState.ErrorOccured => dialog1.ErrorPart,
            _ => throw new NotImplementedException()
        };
        var dialog = DialogFixture.Create(Id, dialog1.Metadata, currentPart);

        // Act
        var result = sut.Continue(dialog); // this will trigger the redirect to dialog 2

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
        var dialogDefinition = DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Test dialog")
                .WithId("Test")
                .WithVersion("1.0.0"))
            .AddParts(welcomePart)
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder())
            .Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinition);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var dialog = sut.Start(dialogDefinition.Metadata); // this will trigger the message

        // Act
        var result = sut.Continue(dialog); // this will trigger the completion

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentPartId.Value.Should().Be("Completed");
    }

    [Fact]
    public void Continue_Throws_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var continuation = new Action(() => sut.Continue(factory.Create(dialogDefinition)));

        // Act
        continuation.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]");
    }

    [Fact]
    public void Continue_Throws_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var continuation = new Action(() => sut.Continue(factory.Create(dialogDefinition)));

        // Act
        continuation.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    [Fact]
    public void CanContinue_Returns_Correct_Result_Based_On_Current_State()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        var sut = CreateSut();

        // Act
        var result = sut.CanContinue(dialog, Enumerable.Empty<IDialogPartResult>());

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public void CanStart_Returns_Correct_Value_Based_On_DialogDefinition_CanStart(bool dialogCanStart, bool expectedResult)
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder().WithFriendlyName("Name").WithCanStart(dialogCanStart).WithId("Id").WithVersion("1.0.0"))
            .AddParts(new MessageDialogPartBuilder().WithId(new DialogPartIdentifierBuilder()).WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder())))
            .Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               dialog => DialogFixture.Create(dialog.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinition);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);

        // Act
        var actual = sut.CanStart(dialogDefinition.Metadata);

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Fact]
    public void CanStart_Returns_False_When_CurrentState_Is_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder().WithFriendlyName("Name").WithCanStart(true).WithId("Id").WithVersion("1.0.0"))
            .AddParts(new MessageDialogPartBuilder().WithId(new DialogPartIdentifierBuilder()).WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder())))
            .Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               dialog => DialogFixture.Create("Id", dialog.Metadata, dialog.ErrorPart)); // dialog state is already in progress
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinition);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);

        // Act
        var actual = sut.CanStart(dialogDefinition.Metadata);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void Start_Throws_When_ContextFactory_CanCreate_Returns_False()
    {
        // Arrange
        var factory = new DialogFactoryFixture(_ => false,
                                               _ => throw new InvalidOperationException("Not intended to get to this point"));
        var repository = new TestDialogDefinitionRepository();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repository, conditionEvaluator, _loggerMock.Object);
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var act = new Action(() => sut.Start(dialogDefinition.Metadata));

        // Act
        act.Should().ThrowExactly<InvalidOperationException>().WithMessage("Could not create dialog");
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
        errorPartMock.Setup(x => x.GetState()).Returns(DialogState.ErrorOccured);
        var dialogDefinitionMock = new Mock<IDialogDefinition>();
        dialogDefinitionMock.SetupGet(x => x.Metadata).Returns(dialogMetadataMock.Object);
        dialogDefinitionMock.SetupGet(x => x.ErrorPart).Returns(errorPartMock.Object);
        dialogDefinitionMock.Setup(x => x.GetFirstPart(It.IsAny<IDialog>(), It.IsAny<IConditionEvaluator>())).Returns(dialogPartMock.Object);
        var factory = new DialogFactoryFixture(_ => true,
                                               _ => DialogFixture.Create("Id", dialogDefinitionMock.Object.Metadata, dialogPartMock.Object));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinitionMock.Object);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var dialog = dialogDefinitionMock.Object;

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
        var dialogDefinition2 = DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 2")
                .WithId("Dialog2")
                .WithVersion("1.0.0"))
            .AddParts(welcomePart)
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder()).Build();
        var redirectPart = new RedirectDialogPartBuilder()
            .WithRedirectDialogMetadata(new DialogMetadataBuilder(dialogDefinition2.Metadata))
            .WithId(new DialogPartIdentifierBuilder().WithValue("Redirect"));
        var dialogDefinition1 = DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 1")
                .WithId("Dialog1")
                .WithVersion("1.0.0"))
            .AddParts(redirectPart)
            .Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition1.Metadata.Id) || Equals(d.Metadata.Id, dialogDefinition2.Metadata.Id),
                                               dialog => Equals(dialog.Metadata.Id, dialogDefinition1.Metadata.Id)
                                                   ? DialogFixture.Create(dialogDefinition1.Metadata)
                                                   : DialogFixture.Create(dialogDefinition2.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns<IDialogDefinitionIdentifier>(identifier =>
        {
            if (Equals(identifier.Id, dialogDefinition1.Metadata.Id) && Equals(identifier.Version, dialogDefinition1.Metadata.Version)) return dialogDefinition1;
            if (Equals(identifier.Id, dialogDefinition2.Metadata.Id) && Equals(identifier.Version, dialogDefinition2.Metadata.Version)) return dialogDefinition2;
            return null;
        });
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);

        // Act
        var result = sut.Start(dialogDefinition1.Metadata);

        // Assert
        result.CurrentState.Should().Be(DialogState.Completed);
        result.CurrentDialogIdentifier.Id.Should().BeEquivalentTo(dialogDefinition1.Metadata.Id);
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
        var dialogDefinition = DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Test dialog")
                .WithId("Test")
                .WithVersion("1.0.0"))
            .AddParts(navigationPart, welcomePart)
            .Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinition);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);

        // Act
        var result = sut.Start(dialogDefinition.Metadata);

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroupId.Should().BeEquivalentTo(welcomePart.Group.Id);
        result.CurrentPartId.Should().BeEquivalentTo(welcomePart.Id);
    }

    [Fact]
    public void Start_Throws_When_Dialog_Could_Not_Be_Created()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => throw new InvalidOperationException("Kaboom"));
        var repository = new TestDialogDefinitionRepository();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repository, conditionEvaluator, _loggerMock.Object);
        var start = new Action(() => sut.Start(dialogDefinition.Metadata));

        // Act
        start.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    [Fact]
    public void Start_Throws_When_DialogDefinition_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var start = new Action(() => sut.Start(dialogDefinition.Metadata));

        // Act
        start.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]");
    }

    [Fact]
    public void Start_Throws_When_DialogDefinition_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var start = new Action(() => sut.Start(dialogDefinition.Metadata));

        // Act
        start.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    [Fact]
    public void Start_Returns_First_DialogPart_When_It_Could_Be_Determined()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = CreateSut();

        // Act
        var result = sut.Start(dialogDefinition.Metadata);

        // Assert
        result.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.Parts.OfType<IGroupedDialogPart>().First().Group.Id);
        result.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.Parts.First().Id);
    }

    [Fact]
    public void Start_Returns_ErrorDialogPart_When_DecisionPart_Returns_ErrorDialogPart()
    {
        // Arrange
        var decisionPart = new DecisionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Decision"))
            .WithDefaultNextPartId(new DialogPartIdentifierBuilder().WithValue("Error"));
        var dialogDefinition = DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Test dialog")
                .WithId("Test")
                .WithVersion("1.0.0"))
            .AddParts(decisionPart)
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder())
            .Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinition);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);

        // Act
        var result = sut.Start(dialogDefinition.Metadata);

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
        var dialogDefinition = DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Test dialog")
                .WithId("Test")
                .WithVersion("1.0.0"))
            .AddParts(decisionPart)
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder())
            .Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinition);
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);

        // Act
        var result = sut.Start(dialogDefinition.Metadata);

        // Assert
        result.CurrentState.Should().Be(DialogState.Aborted);
        result.CurrentGroupId.Should().BeNull();
        result.CurrentPartId.Value.Should().Be("Abort");
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Parts_Does_Not_Contain_Current_Part()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var completedPart = dialogDefinition.CompletedPart;
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(dialog, completedPart.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Requested_Part_Is_Not_Part_Of_Current_Dialog()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var errorPart = dialogDefinition.ErrorPart;
        var completedPart = dialogDefinition.CompletedPart;
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, errorPart);
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(dialog, completedPart.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Requested_Part_Is_After_Current_Part()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var messagePart = dialogDefinition.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, messagePart);
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(dialog, questionPart.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_Part_Is_Current_Part()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(dialog, questionPart.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_Requested_Part_Is_Before_Current_Part()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var messagePart = dialogDefinition.Parts.OfType<IMessageDialogPart>().First();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        IDialog dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, messagePart);
        var partResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(messagePart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(string.Empty))
            .Build();
        dialog = dialog.Chain(x => x.Continue(dialogDefinition, new[] { partResult }, conditionEvaluatorMock.Object));
        var sut = CreateSut();

        // Act
        var result = sut.CanNavigateTo(dialog, messagePart.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void NavigateTo_Returns_Error_When_CanNavigateTo_Is_False()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var messagePart = dialogDefinition.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, messagePart);
        var sut = CreateSut();

        // Act
        var result = sut.NavigateTo(dialog, questionPart.Id);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
    }

    [Fact]
    public void NavigateTo_Navigates_To_Requested_Part_When_CanNavigate_Is_True()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var messagePart = dialogDefinition.Parts.OfType<IMessageDialogPart>().First();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        IDialog dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, messagePart);
        var partResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(messagePart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(string.Empty))
            .Build();
        dialog = dialog.Chain(x => x.Continue(dialogDefinition, new[] { partResult }, conditionEvaluatorMock.Object));
        var sut = CreateSut();

        // Act
        var result = sut.NavigateTo(dialog, messagePart.Id);

        // Assert
        result.CurrentState.Should().Be(DialogState.InProgress);
        result.CurrentGroupId.Should().BeEquivalentTo(messagePart.Group.Id);
        result.CurrentPartId.Should().BeEquivalentTo(messagePart.Id);
    }

    [Fact]
    public void NavigateTo_Throws_When_DialogDefinition_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var navigate = new Action(() => sut.NavigateTo(factory.Create(dialogDefinition), dialogDefinition.Parts.First().Id));

        // Act
        navigate.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]");
    }

    [Fact]
    public void NavigateTo_Throws_When_DialogDefinition_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var navigate = new Action(() => sut.NavigateTo(factory.Create(dialogDefinition), dialogDefinition.Parts.First().Id));

        // Act
        navigate.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    [Fact]
    public void CanResetCurrentState_Returns_False_When_CurrentState_Is_Not_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.AbortedPart);
        var sut = CreateSut();

        // Act
        var result = sut.CanResetCurrentState(dialog);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanResetCurrentState_Returns_False_When_Current_DialogPart_Is_Not_QuestionDialogPart()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.CompletedPart); // note that this actually invalid state, but we currently can't prevent it on the interface
        var sut = CreateSut();

        // Act
        var result = sut.CanResetCurrentState(dialog);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanResetCurrentState_Returns_True_When_CurrentState_Is_InProgress_And_Current_DialogPart_Is_QuestionDialogPart()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        var sut = CreateSut();

        // Act
        var result = sut.CanResetCurrentState(dialog);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ResetCurrentState_Resets_Answers_From_Current_Question_When_All_Is_Good()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        dialog = DialogFixture.Create(dialog, new[]
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
        var result = sut.ResetCurrentState(dialog);

        // Assert
        var dialogPartResults = result.Results;
        dialogPartResults.Should().ContainSingle();
        dialogPartResults.Single().DialogPartId.Value.Should().Be("Other part");
    }

    [Fact]
    public void ResetCurrentState_Returns_ErrorDialogPart_When_CanResetCurrentState_Is_False()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.AbortedPart);
        var sut = CreateSut();

        // Act
        var result = sut.ResetCurrentState(dialog);

        // Assert
        result.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.CurrentGroupId.Should().BeNull();
        result.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.ErrorPart.Id);
    }

    [Fact]
    public void ResetCurrentState_Throws_When_DialogDefinition_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var resetCurrentState = new Action(() => sut.ResetCurrentState(factory.Create(dialogDefinition)));

        // Act
        resetCurrentState.Should().ThrowExactly<InvalidOperationException>().WithMessage("Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]");
    }

    [Fact]
    public void ResetCurrentState_Throws_When_DialogDefinition_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        var sut = new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
        var resetCurrentState = new Action(() => sut.ResetCurrentState(factory.Create(dialogDefinition)));

        // Act
        resetCurrentState.Should().ThrowExactly<InvalidOperationException>().WithMessage("Kaboom");
    }

    private DialogService CreateSut()
    {
        var factory = new DialogFactory();
        var repository = new TestDialogDefinitionRepository();
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        return new DialogService(factory, repository, conditionEvaluator, _loggerMock.Object);
    }

    private DialogService CreateSutForTwoDialogsWithRedirect(out IDialogDefinition dialogDefinition1)
    {
        var welcomePart = new MessageDialogPartBuilder()
            .WithMessage("Welcome! I would like to answer a question")
            .WithGroup(DialogPartGroupFixture.CreateBuilder())
            .WithHeading("Welcome")
            .WithId(new DialogPartIdentifierBuilder().WithValue("Welcome"));
        var dialogDefinition2 = DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 2")
                .WithId("Dialog2")
                .WithVersion("1.0.0"))
            .AddParts(welcomePart)
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder())
            .Build();
        var redirectPart = new RedirectDialogPartBuilder()
            .WithRedirectDialogMetadata(new DialogMetadataBuilder(dialogDefinition2.Metadata))
            .WithId(new DialogPartIdentifierBuilder().WithValue("Redirect"));
        dialogDefinition1 = DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 1")
                .WithId("Dialog1")
                .WithVersion("1.0.0"))
            .AddParts(welcomePart, redirectPart)
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder())
            .Build();
        var id1 = dialogDefinition1.Metadata.Id;
        var metadata1 = dialogDefinition1.Metadata;
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, id1),
                                               _ => DialogFixture.Create(metadata1));
        var repositoryMock = new Mock<IDialogDefinitionRepository>();
        var d1 = dialogDefinition1;
        var d2 = dialogDefinition2;
        repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns<IDialogDefinitionIdentifier>(identifier =>
        {
            if (Equals(identifier.Id, "Dialog1")) return d1;
            if (Equals(identifier.Id, "Dialog2")) return d2;
            return null;
        });
        var conditionEvaluator = new Mock<IConditionEvaluator>().Object;
        return new DialogService(factory, repositoryMock.Object, conditionEvaluator, _loggerMock.Object);
    }
}
