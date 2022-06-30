namespace DialogFramework.Application.Tests.RequestHandlers;

public class StartRequestHandlerTests : RequestHandlerTestBase
{
    public StartRequestHandlerTests() : base() { }

    [Fact]
    public void Handle_Returns_Error_When_ContextFactory_Returns_NonSuccess()
    {
        // Arrange
        var factory = new DialogFactoryFixture(_ => false,
                                               _ => throw new InvalidOperationException("Not intended to get to this point"));
        var provider = new TestDialogDefinitionProvider();
        var sut = new StartRequestHandler(factory, provider, ConditionEvaluatorMock.Object, LoggerMock.Object);
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var result = sut.Handle(new StartRequest(dialogDefinition.Metadata));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Dialog creation failed");
    }

    [Fact]
    public void Handle_Returns_Invalid_When_CanHandle_Is_False()
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
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(Result<IDialogDefinition>.Success(dialogDefinitionMock.Object));
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);
        var dialog = dialogDefinitionMock.Object;

        // Act
        var result = sut.Handle(new StartRequest(dialog.Metadata));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Dialog definition cannot be started");
    }

    [Fact]
    public void Handle_Returns_Ok_And_Puts_Dialog_In_ErrorState_When_Dialog_Handle_Throws()
    {
        // Arrange
        var definition = DialogDefinitionFixture.CreateBuilderBase()
            .AddParts(DialogPartFixture.CreateErrorThrowingDialogPartBuilder())
            .Build();
        var dialog = DialogFixture.Create(definition.Metadata);
        var factory = new DialogFactoryFixture(_ => true, _ => dialog);
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>()))
                       .Returns(Result<IDialogDefinition>.Success(definition));
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = sut.Handle(new StartRequest(definition.Metadata));

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.IsSuccessful().Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value!.CurrentPartId.Should().BeEquivalentTo(definition.ErrorPart.Id);
        result.Value!.ErrorMessage.Should().Be("Start failed");
    }

    [Fact]
    public void Handle_Uses_Result_From_RedirectPart()
    {
        // Arrange
        var dialogDefinition2 = CreateSecondDialogDefinition();
        var dialogDefinition1 = CreateFirstDialogDefinition(dialogDefinition2);
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition1.Metadata.Id) || Equals(d.Metadata.Id, dialogDefinition2.Metadata.Id),
                                               dialog => Equals(dialog.Metadata.Id, dialogDefinition1.Metadata.Id)
                                                   ? DialogFixture.Create(dialogDefinition1.Metadata)
                                                   : DialogFixture.Create(dialogDefinition2.Metadata));
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns<IDialogDefinitionIdentifier>(identifier =>
        {
            if (Equals(identifier.Id, dialogDefinition1.Metadata.Id) && Equals(identifier.Version, dialogDefinition1.Metadata.Version)) return Result<IDialogDefinition>.Success(dialogDefinition1);
            if (Equals(identifier.Id, dialogDefinition2.Metadata.Id) && Equals(identifier.Version, dialogDefinition2.Metadata.Version)) return Result<IDialogDefinition>.Success(dialogDefinition2);
            return Result<IDialogDefinition>.NotFound();
        });
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = sut.Handle(new StartRequest(dialogDefinition1.Metadata));

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.InProgress);
        result.Value!.CurrentDialogIdentifier.Id.Should().BeEquivalentTo(dialogDefinition2.Metadata.Id);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition2.Parts.First().Id);
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition2.Parts.First().GetGroup()!.Id);
    }

    [Fact]
    public void Handle_Uses_Result_From_NavigationPart()
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
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(Result<IDialogDefinition>.Success(dialogDefinition));
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = sut.Handle(new StartRequest(dialogDefinition.Metadata));

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.InProgress);
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(welcomePart.Group.Id);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(welcomePart.Id);
    }

    [Fact]
    public void Handle_Returns_Error_When_Dialog_Could_Not_Be_Created()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => throw new InvalidOperationException("Kaboom"));
        var provider = new TestDialogDefinitionProvider();
        var sut = new StartRequestHandler(factory, provider, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = sut.Handle(new StartRequest(dialogDefinition.Metadata));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Dialog creation failed");
        AssertLogging("CreateDialogAndDefinition failed", "Kaboom");
    }

    [Fact]
    public void Handle_Returns_NotFound_When_DialogDefinition_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>()))
                     .Returns(Result<IDialogDefinition>.NotFound());
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = sut.Handle(new StartRequest(dialogDefinition.Metadata));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        var msg = "Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]";
        result.ErrorMessage.Should().Be(msg);
        AssertLogging(msg, null);
    }

    [Fact]
    public void Handle_Returns_Error_When_DialogDefinition_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = sut.Handle(new StartRequest(dialogDefinition.Metadata));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
    }

    [Fact]
    public void Handle_Returns_First_DialogPart_When_It_Could_Be_Determined()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = new StartRequestHandler(
            new DialogFactory(),
            new TestDialogDefinitionProvider(),
            ConditionEvaluatorMock.Object,
            LoggerMock.Object);

        // Act
        var result = sut.Handle(new StartRequest(dialogDefinition.Metadata));

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.Parts.OfType<IGroupedDialogPart>().First().Group.Id);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.Parts.First().Id);
    }

    [Fact]
    public void Handle_Returns_ErrorDialogPart_When_DecisionPart_Returns_ErrorDialogPart()
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
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(Result<IDialogDefinition>.Success(dialogDefinition));
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = sut.Handle(new StartRequest(dialogDefinition.Metadata));

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().NotBeNull();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.CurrentPartId.Value.Should().Be("Error");
    }

    [Fact]
    public void Handle_Returns_AbortDialogPart_When_DecisionPart_Returns_AbortDialogPart()
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
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(Result<IDialogDefinition>.Success(dialogDefinition));
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = sut.Handle(new StartRequest(dialogDefinition.Metadata));

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.Aborted);
        result.Value!.CurrentPartId.Value.Should().Be("Abort");
    }

    [Fact]
    public void Handle_Fills_Results_From_Previous_Session_On_Dialog_When_Present()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = new StartRequestHandler(
            new DialogFactory(),
            new TestDialogDefinitionProvider(),
            ConditionEvaluatorMock.Object,
            LoggerMock.Object);
        var partResult = new DialogPartResultBuilder()
            .WithDialogId(new DialogDefinitionIdentifierBuilder(dialogDefinition.Metadata))
            .WithDialogPartId(new DialogPartIdentifierBuilder(dialogDefinition.Parts.First().Id))
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var result = sut.Handle(new StartRequest(dialogDefinition.Metadata, new[] { partResult }));

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().NotBeNull();
        result.GetValueOrThrow().GetAllResults(dialogDefinition).Should().ContainSingle();
    }

    private static IDialogDefinition CreateFirstDialogDefinition(IDialogDefinition dialogDefinition2)
        => DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 1")
                .WithId("Dialog1")
                .WithVersion("1.0.0"))
            .AddParts(new RedirectDialogPartBuilder()
                .WithRedirectDialogMetadata(new DialogMetadataBuilder(dialogDefinition2.Metadata))
                .WithId(new DialogPartIdentifierBuilder().WithValue("Redirect")))
            .Build();

    private static IDialogDefinition CreateSecondDialogDefinition()
        => DialogDefinitionFixture.CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 2")
                .WithId("Dialog2")
                .WithVersion("1.0.0"))
            .AddParts(new MessageDialogPartBuilder()
                .WithMessage("Welcome! I would like to answer a question")
                .WithGroup(DialogPartGroupFixture.CreateBuilder())
                .WithId(new DialogPartIdentifierBuilder().WithValue("Welcome"))
                .WithHeading("Welcome"))
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder()).Build();
}
