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
    public void Abort_Returns_ErrorDialogPart_When_Validation_Fails()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var abortedPart = dialogDefinition.AbortedPart;
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, abortedPart);
        var sut = CreateSut();

        // Act
        var result = sut.Abort(dialog);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.ErrorPart.Id);
        result.Value!.Errors.Select(x => x.Message).Should().BeEquivalentTo(new[] { "Current state is invalid" });
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
        result.Value!.CurrentState.Should().Be(DialogState.Aborted);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(abortedPart.Id);
    }

    [Fact]
    public void Abort_Returns_ErrorMessage_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);
        var result = sut.Abort(factory.Create(dialogDefinition));

        // Act
        result.IsSuccessful().Should().BeFalse();
        var msg = "Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]";
        result.ErrorMessage.Should().Be(msg);
        AssertLogging(msg, null);
    }

    [Fact]
    public void Abort_Returns_ErrorMessage_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);
        var result = sut.Abort(factory.Create(dialogDefinition));

        // Act
        result.IsSuccessful().Should().BeFalse();
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
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
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.ErrorPart.Id);
        result.Value!.Errors.Select(x => x.Message).Should().BeEquivalentTo(new[] { "Current state is invalid" });
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
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.Completed);
        result.Value!.CurrentPartId.Value.Should().Be("Completed");
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.CompletedPart.Group.Id);
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
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.InProgress);
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(currentPart.GetGroupId());
        result.Value!.ValidationErrors.Should().ContainSingle();
        result.Value!.ValidationErrors.Single().ErrorMessage.Should().Be("Unknown Result Id: [DialogPartResultIdentifier { Value = Unknown result }]");
    }

    [Fact]
    public void Continue_Uses_Result_From_DecisionPart()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var currentPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().First();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(Id, dialogDefinition.Metadata, currentPart));
        var repository = new TestDialogDefinitionRepository();
        var sut = new DialogApplicationService(factory, repository, _conditionEvaluatorMock.Object, _loggerMock.Object);
        var dialog = sut.Continue(factory.Create(dialogDefinition)).Value!; // skip the welcome message
        var dialogPartResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(currentPart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Terrible"))
            .Build();

        // Act
        var result = sut.Continue(dialog, new[] { dialogPartResult }); // answer the question with 'Terrible', this will trigger a second message

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.Completed);
        result.Value!.CurrentPartId.Value.Should().Be("Completed");
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.CompletedPart.Group.Id);
    }

    [Fact]
    public void Continue_Returns_ErrorDialogPart_On_RedirectPart()
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
        var sut = new DialogApplicationService(factory, repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);
        var dialog = sut.Start(dialogDefinition1.Metadata.Build()).Value!; // this will trigger the message on dialog 1

        // Act
        var result = sut.Continue(dialog); // this will trigger the redirect to dialog 2

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition1.ErrorPart.Id);
        result.Value!.Errors.Select(x => x.Message).Should().BeEquivalentTo(new[] { "Current state is invalid" });
    }

    [Fact]
    public void Continue_Returns_Error_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);
        var result = sut.Continue(factory.Create(dialogDefinition));

        // Act
        result.IsSuccessful().Should().BeFalse();
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
        var result = sut.Continue(factory.Create(dialogDefinition));

        // Act
        result.IsSuccessful().Should().BeFalse();
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public void Start_Returns_Correct_Result_Based_On_DialogDefinition_CanStart(bool dialogCanStart, bool expectedResult)
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder().WithFriendlyName("Name").WithCanStart(dialogCanStart).WithId("Id").WithVersion("1.0.0"))
            .AddParts(new MessageDialogPartBuilder().WithId(new DialogPartIdentifierBuilder()).WithGroup(new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder())))
            .Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               dialog => DialogFixture.Create(dialog.Metadata));
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinition);
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);

        // Act
        var result = sut.Start(dialogDefinition.Metadata);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        if (expectedResult)
        {
            result.Value!.CurrentState.Should().Be(DialogState.InProgress);
        }
        else
        {
            result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
            result.Value!.Errors.Select(x => x.Message).Should().BeEquivalentTo(new[] { "Dialog definition cannot be started" });
        }
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
        var result = sut.Start(dialogDefinition.Metadata);

        // Act
        result.IsSuccessful().Should().BeFalse();
        result.ErrorMessage.Should().Be("Could not create dialog");
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
        dialogDefinitionMock.Setup(x => x.GetFirstPart(It.IsAny<IDialog>(), It.IsAny<IConditionEvaluator>())).Returns(Result<IDialogPart>.Success(dialogPartMock.Object));
        var factory = new DialogFactoryFixture(_ => true,
                                               _ => DialogFixture.Create("Id", dialogDefinitionMock.Object.Metadata, dialogPartMock.Object));
        _repositoryMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(dialogDefinitionMock.Object);
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);
        var dialog = dialogDefinitionMock.Object;

        // Act
        var result = sut.Start(dialog.Metadata);

        // Act
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.Errors.Select(x => x.Message).Should().BeEquivalentTo(new[] { "Dialog definition cannot be started" });
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
        var result = sut.Start(dialogDefinition.Metadata);

        // Act
        result.IsSuccessful().Should().BeFalse();
        result.ErrorMessage.Should().Be("Dialog creation failed");
        AssertLogging("Start failed", "Kaboom");
    }

    [Fact]
    public void Start_Returns_Error_When_DialogDefinition_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);
        var result = sut.Start(dialogDefinition.Metadata);

        // Act
        result.IsSuccessful().Should().BeFalse();
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
        var result = sut.Start(dialogDefinition.Metadata);

        // Act
        result.IsSuccessful().Should().BeFalse();
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
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.CurrentGroupId.Should().BeNull();
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
        result.Value!.CurrentState.Should().Be(DialogState.Aborted);
        result.Value!.CurrentGroupId.Should().BeNull();
        result.Value!.CurrentPartId.Value.Should().Be("Abort");
    }

    [Fact]
    public void NavigateTo_Returns_ErrorDialogPart_When_Parts_Does_Not_Contain_Current_Part()
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
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.ErrorPart.Id);
        result.Value!.Errors.Select(x => x.Message).Should().BeEquivalentTo(new[] { "Cannot navigate to the specified part" });
    }

    [Fact]
    public void NavigateTo_Returns_ErrorDialogPart_When_Requested_Part_Is_Not_Part_Of_Current_Dialog()
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
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.ErrorPart.Id);
        result.Value!.Errors.Select(x => x.Message).Should().BeEquivalentTo(new[] { "Current state is invalid" });
    }

    [Fact]
    public void NavigateTo_Returns_ErrorDialogPart_When_Requested_Part_Is_After_Current_Part()
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
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.ErrorPart.Id);
        result.Value!.Errors.Select(x => x.Message).Should().BeEquivalentTo(new[] { "Cannot navigate to the specified part" });
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
    }

    [Fact]
    public void NavigateTo_Returns_Success_When_Requested_Part_Is_Before_Current_Part()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var messagePart = dialogDefinition.Parts.OfType<IMessageDialogPart>().First();
        IDialog dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, messagePart);
        var partResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(messagePart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(string.Empty))
            .Build();
        dialog.Continue(dialogDefinition, new[] { partResult }, _conditionEvaluatorMock.Object);
        var sut = CreateSut();

        // Act
        var result = sut.NavigateTo(dialog, messagePart.Id);

        // Assert
        result.IsSuccessful().Should().BeTrue();
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
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
    }

    [Fact]
    public void NavigateTo_Navigates_To_Requested_Part_When_CanNavigate_Is_True()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var messagePart = dialogDefinition.Parts.OfType<IMessageDialogPart>().First();
        IDialog dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, messagePart);
        var partResult = new DialogPartResultBuilder()
            .WithDialogPartId(new DialogPartIdentifierBuilder(messagePart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(string.Empty))
            .Build();
        dialog.Continue(dialogDefinition, new[] { partResult }, _conditionEvaluatorMock.Object);
        var sut = CreateSut();

        // Act
        var result = sut.NavigateTo(dialog, messagePart.Id);

        // Assert
        result.IsSuccessful().Should().BeTrue();
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
        var result = sut.NavigateTo(factory.Create(dialogDefinition), dialogDefinition.Parts.First().Id);

        // Act
        result.IsSuccessful().Should().BeFalse();
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
        var result = sut.NavigateTo(factory.Create(dialogDefinition), dialogDefinition.Parts.First().Id);

        // Act
        result.IsSuccessful().Should().BeFalse();
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
    }

    [Fact]
    public void ResetCurrentState_Returns_ErrorDialogPart_When_CurrentState_Is_Not_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.AbortedPart);
        var sut = CreateSut();

        // Act
        var result = sut.ResetCurrentState(dialog);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.ErrorPart.Id);
        result.Value!.Errors.Select(x => x.Message).Should().BeEquivalentTo(new[] { "Current state is invalid" });
    }

    [Fact]
    public void ResetCurrentState_Returns_ErrorDialogPart_When_Current_DialogPart_Is_Not_QuestionDialogPart()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.CompletedPart); // note that this actually invalid state, but we currently can't prevent it on the interface
        var sut = CreateSut();

        // Act
        var result = sut.ResetCurrentState(dialog);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.ErrorPart.Id);
        result.Value!.Errors.Select(x => x.Message).Should().BeEquivalentTo(new[] { "Current state is invalid" });
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
        var dialogPartResults = result.Value!.Results;
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
        result.IsSuccessful().Should().BeTrue();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.CurrentGroupId.Should().BeNull();
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.ErrorPart.Id);
    }

    [Fact]
    public void ResetCurrentState_Returns_Error_When_DialogDefinition_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        var sut = new DialogApplicationService(factory, _repositoryMock.Object, _conditionEvaluatorMock.Object, _loggerMock.Object);
        var result = sut.ResetCurrentState(factory.Create(dialogDefinition));

        // Act
        result.IsSuccessful().Should().BeFalse();
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
        var result = sut.ResetCurrentState(factory.Create(dialogDefinition));

        // Act
        result.IsSuccessful().Should().BeFalse();
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
