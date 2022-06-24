namespace DialogFramework.Application.Tests;

public class DialogApplicationServiceTests
{
    private readonly Mock<ILogger> _loggerMock;
    private readonly Mock<IDialogDefinitionRepository> _repositoryMock;
    private readonly Mock<IConditionEvaluator> _conditionEvaluatorMock;

    public DialogApplicationServiceTests()
    {
        _loggerMock = new Mock<ILogger>();
        _repositoryMock = new Mock<IDialogDefinitionRepository>();
        _conditionEvaluatorMock = new Mock<IConditionEvaluator>();
    }

    private static string Id => Guid.NewGuid().ToString();

    [Fact]
    public void Abort_Returns_Invalid_When_Validation_Fails()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var abortedPart = dialogDefinition.AbortedPart;
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, abortedPart);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(dialog);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public void Abort_Returns_AbortDialogPart_Dialog_When_Validation_Succeeds()
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
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.Aborted);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(abortedPart.Id);
    }

    [Fact]
    public void Abort_Returns_NotFound_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.Abort(factory.Create(dialogDefinition));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        var msg = "Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]";
        result.ErrorMessage.Should().Be(msg);
        AssertLogging(msg, null);
    }

    [Fact]
    public void Abort_Returns_Error_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.Abort(factory.Create(dialogDefinition));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
    }

    [Theory]
    [InlineData(DialogState.Aborted)]
    [InlineData(DialogState.Completed)]
    [InlineData(DialogState.ErrorOccured)]
    public void Continue_Returns_Invalid_On_Invalid_State(DialogState currentState)
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
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public void Continue_Returns_Next_DialogPart_When_Current_State_Is_Question_And_Answer_Is_Valid()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var currentPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().First();
        var definition = DialogFixture.Create(Id, dialogDefinition.Metadata, currentPart);
        var sut = CreateSut();
        var dialogPartResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Great"))
            .Build();

        // Act
        var result = sut.Continue(definition, new[] { dialogPartResult });

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.Completed);
        result.Value!.CurrentPartId.Value.Should().Be("Completed");
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.CompletedPart.Group.Id);
    }

    [Fact]
    public void Continue_Returns_Invalid_When_Current_State_Is_Question_And_Answer_Is_Not_Valid()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var currentPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().First();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, currentPart);
        var sut = CreateSut();
        var dialogPartResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Unknown result"))
            .Build();

        // Act
        var result = sut.Continue(dialog, new[] { dialogPartResult });

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Validation failed, see ValidationErrors for more details");
        result.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[] { "Unknown Result Id: [DialogPartResultIdentifier { Value = Unknown result }]" });
    }

    [Fact]
    public void Continue_Returns_NotFound_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.Continue(factory.Create(dialogDefinition));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        var msg = "Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]";
        result.ErrorMessage.Should().Be(msg);
        AssertLogging(msg, null);
    }

    [Fact]
    public void Continue_Returns_Error_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.Continue(factory.Create(dialogDefinition));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
    }

    [Fact]
    public void Start_Returns_Error_When_ContextFactory_CanCreate_Returns_False()
    {
        // Arrange
        var factory = new DialogFactoryFixture(_ => false,
                                               _ => throw new InvalidOperationException("Not intended to get to this point"));
        var repository = new TestDialogDefinitionRepository();
        var sut = new DialogApplicationService(factory, repository, _conditionEvaluatorMock.Object, _loggerMock.Object);
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var result = sut.Start(dialogDefinition.Metadata);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Could not create dialog");
    }

    [Fact]
    public void Start_Returns_Invalid_When_CanStart_Is_False()
    {
        // Arrange
        var dialogMetadataMock = new Mock<IDialogMetadata>();
        dialogMetadataMock.SetupGet(x => x.CanStart).Returns(false);
        dialogMetadataMock.SetupGet(x => x.Id).Returns("Empty");
        var dialogPartMock = new Mock<IDialogPart>();
        dialogPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifierBuilder().Build());
        var errorPartMock = new Mock<IErrorDialogPart>();
        errorPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifierBuilder().WithValue("Error").Build());
        errorPartMock.Setup(x => x.GetState()).Returns(DialogState.ErrorOccured);
        var dialogDefinitionMock = new Mock<IDialogDefinition>();
        dialogDefinitionMock.SetupGet(x => x.Metadata).Returns(dialogMetadataMock.Object);
        dialogDefinitionMock.SetupGet(x => x.ErrorPart).Returns(errorPartMock.Object);
        dialogDefinitionMock.Setup(x => x.GetFirstPart(It.IsAny<IDialog>(), It.IsAny<IConditionEvaluator>())).Returns(Result<IDialogPart>.Success(dialogPartMock.Object));
        var factory = new DialogFactoryFixture(_ => true,
                                               _ => DialogFixture.Create("Id", dialogDefinitionMock.Object.Metadata, dialogPartMock.Object));
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinitionMock.Object);
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);
        var dialog = dialogDefinitionMock.Object;

        // Act
        var result = sut.Start(dialog.Metadata);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Dialog definition cannot be started");
    }

    [Fact]
    public void Start_Returns_Ok_And_Puts_Dialog_In_ErrorState_When_Dialog_Start_Throws()
    {
        // Arrange
        var definition = DialogDefinitionFixture.CreateBuilderBase()
            .AddParts(DialogPartFixture.CreateErrorThrowingDialogPartBuilder())
            .Build();
        var dialog = DialogFixture.Create(definition.Metadata);
        var factory = new DialogFactoryFixture(_ => true, _ => dialog);
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>()))
                       .Returns(definition);
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.Start(definition.Metadata);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.IsSuccessful().Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value!.CurrentPartId.Should().BeEquivalentTo(definition.ErrorPart.Id);
        result.Value!.ErrorMessage.Should().Be("Start failed");
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
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns<IDialogDefinitionIdentifier>(identifier =>
        {
            if (Equals(identifier.Id, dialogDefinition1.Metadata.Id) && Equals(identifier.Version, dialogDefinition1.Metadata.Version)) return dialogDefinition1;
            if (Equals(identifier.Id, dialogDefinition2.Metadata.Id) && Equals(identifier.Version, dialogDefinition2.Metadata.Version)) return dialogDefinition2;
            return null;
        });
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.Start(dialogDefinition1.Metadata);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.Completed);
        result.Value!.CurrentDialogIdentifier.Id.Should().BeEquivalentTo(dialogDefinition1.Metadata.Id);
        result.Value!.CurrentGroupId.Should().BeNull();
        result.Value!.CurrentPartId.Value.Should().Be("Redirect");
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
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinition);
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.Start(dialogDefinition.Metadata);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.InProgress);
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(welcomePart.Group.Id);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(welcomePart.Id);
    }

    [Fact]
    public void Start_Returns_Error_When_Dialog_Could_Not_Be_Created()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => throw new InvalidOperationException("Kaboom"));
        var repository = new TestDialogDefinitionRepository();
        var sut = new DialogApplicationService(factory, repository, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.Start(dialogDefinition.Metadata);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Dialog creation failed");
        AssertLogging("Start failed", "Kaboom");
    }

    [Fact]
    public void Start_Returns_NotFound_When_DialogDefinition_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.Start(dialogDefinition.Metadata);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        var msg = "Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]";
        result.ErrorMessage.Should().Be(msg);
        AssertLogging(msg, null);
    }

    [Fact]
    public void Start_Returns_Error_When_DialogDefinition_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.Start(dialogDefinition.Metadata);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
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
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.Parts.OfType<IGroupedDialogPart>().First().Group.Id);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.Parts.First().Id);
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
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinition);
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.Start(dialogDefinition.Metadata);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().NotBeNull();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.CurrentPartId.Value.Should().Be("Error");
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
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinition);
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.Start(dialogDefinition.Metadata);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.Aborted);
        result.Value!.CurrentPartId.Value.Should().Be("Abort");
    }

    [Fact]
    public void NavigateTo_Returns_Invalid_When_Parts_Does_Not_Contain_Current_Part()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var completedPart = dialogDefinition.CompletedPart;
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        var sut = CreateSut();

        // Act
        var result = sut.NavigateTo(dialog, completedPart.Id);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Cannot navigate to the specified part");
    }

    [Fact]
    public void NavigateTo_Returns_Invalid_When_Requested_Part_Is_Not_Part_Of_Current_Dialog()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var errorPart = dialogDefinition.ErrorPart;
        var completedPart = dialogDefinition.CompletedPart;
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, errorPart);
        var sut = CreateSut();

        // Act
        var result = sut.NavigateTo(dialog, completedPart.Id);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public void NavigateTo_Returns_Invalid_When_Requested_Part_Is_After_Current_Part()
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
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Cannot navigate to the specified part");
    }

    [Fact]
    public void NavigateTo_Returns_Success_When_Requested_Part_Is_Current_Part()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        var sut = CreateSut();

        // Act
        var result = sut.NavigateTo(dialog, questionPart.Id);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public void NavigateTo_Returns_Success_When_Requested_Part_Is_Before_Current_Part()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var messagePart = dialogDefinition.Parts.OfType<IMessageDialogPart>().First();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, messagePart);
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(string.Empty))
            .Build();
        dialog.Continue(dialogDefinition, new[] { partResult }, _conditionEvaluatorMock.Object);
        var sut = CreateSut();

        // Act
        var result = sut.NavigateTo(dialog, messagePart.Id);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public void NavigateTo_Navigates_To_Requested_Part_When_CanNavigate_Is_True()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var messagePart = dialogDefinition.Parts.OfType<IMessageDialogPart>().First();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, messagePart);
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(string.Empty))
            .Build();
        dialog.Continue(dialogDefinition, new[] { partResult }, _conditionEvaluatorMock.Object);
        var sut = CreateSut();

        // Act
        var result = sut.NavigateTo(dialog, messagePart.Id);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.InProgress);
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(messagePart.Group.Id);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(messagePart.Id);
    }

    [Fact]
    public void NavigateTo_Returns_Error_When_DialogDefinition_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.NavigateTo(factory.Create(dialogDefinition), dialogDefinition.Parts.First().Id);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        var msg = "Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]";
        result.ErrorMessage.Should().Be(msg);
        AssertLogging(msg, null);
    }

    [Fact]
    public void NavigateTo_Returns_Error_When_DialogDefinition_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.NavigateTo(factory.Create(dialogDefinition), dialogDefinition.Parts.First().Id);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
    }

    [Fact]
    public void ResetCurrentState_Returns_Invalid_When_CurrentState_Is_Not_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.AbortedPart);
        var sut = CreateSut();

        // Act
        var result = sut.ResetCurrentState(dialog);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public void ResetCurrentState_Returns_Invalid_When_Current_DialogPart_Is_Not_QuestionDialogPart()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.CompletedPart); // note that this actually invalid state, but we currently can't prevent it on the interface
        var sut = CreateSut();

        // Act
        var result = sut.ResetCurrentState(dialog);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public void ResetCurrentState_Resets_Answers_From_Current_Question_When_Possible()
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
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        var dialogPartResults = dialog.GetDialogPartResultsByPartIdentifier(questionPart.Id).GetValueOrThrow();
        dialogPartResults.Should().BeEmpty();
    }

    [Fact]
    public void ResetCurrentState_Returns_Error_When_DialogDefinition_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.ResetCurrentState(factory.Create(dialogDefinition));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        var msg = "Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]";
        result.ErrorMessage.Should().Be(msg);
        AssertLogging(msg, null);
    }

    [Fact]
    public void ResetCurrentState_Returns_Error_When_DialogDefinition_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.ResetCurrentState(factory.Create(dialogDefinition));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
    }

    private DialogApplicationService CreateSut()
        => new DialogApplicationService(
            new DialogFactory(),
            new TestDialogDefinitionRepository(),
            _conditionEvaluatorMock.Object,
            _loggerMock.Object);

    private void AssertLogging(string title, string? exceptionMessage)
    {
        if (exceptionMessage != null)
        {
            _loggerMock.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, @type) => @object != null && @object.ToString() == title && @type != null && @type.Name == "FormattedLogValues"),
                    It.Is<InvalidOperationException>(ex => ex.Message == exceptionMessage),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
        else
        {
            _loggerMock.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, @type) => @object != null && @object.ToString() == title && @type != null && @type.Name == "FormattedLogValues"),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
