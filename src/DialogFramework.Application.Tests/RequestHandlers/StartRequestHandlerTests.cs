namespace DialogFramework.Application.Tests.RequestHandlers;

public class StartRequestHandlerTests : RequestHandlerTestBase
{
    public StartRequestHandlerTests() : base() { }

    [Fact]
    public async Task Handle_Returns_Error_When_ContextFactory_Returns_NonSuccess()
    {
        // Arrange
        var factory = new DialogFactoryFixture(_ => false,
                                               _ => throw new InvalidOperationException("Not intended to get to this point"));
        var provider = new TestDialogDefinitionProvider();
        var sut = new StartRequestHandler(factory, provider, ConditionEvaluatorMock.Object, LoggerMock.Object);
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();

        // Act
        var result = await sut.Handle(new StartRequest(dialogDefinition.Metadata), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Dialog creation failed");
    }

    [Fact]
    public async Task Handle_Returns_Invalid_When_CanHandle_Is_False()
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
        dialogDefinitionMock.Setup(x => x.GetFirstPart()).Returns(Result<IDialogPart>.Success(dialogPartMock.Object));
        var factory = new DialogFactoryFixture(_ => true,
                                               _ => DialogFixture.Create("Id", dialogDefinitionMock.Object.Metadata, dialogPartMock.Object));
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Returns(Result<IDialogDefinition>.Success(dialogDefinitionMock.Object));
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);
        var dialog = dialogDefinitionMock.Object;

        // Act
        var result = await sut.Handle(new StartRequest(dialog.Metadata), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Dialog definition cannot be started");
    }

    [Fact]
    public async Task Handle_Returns_Ok_And_Puts_Dialog_In_ErrorState_When_Dialog_Handle_Throws()
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
        var result = await sut.Handle(new StartRequest(definition.Metadata), CancellationToken.None);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.IsSuccessful().Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value!.CurrentPartId.Should().BeEquivalentTo(definition.ErrorPart.Id);
        result.Value!.ErrorMessage.Should().Be("Start failed");
    }

    [Fact]
    public async Task Handle_Uses_Result_From_RedirectPart()
    {
        // Arrange
        var dialogDefinition2 = DialogDefinitionFixture.CreateSecondDialogDefinition();
        var dialogDefinition1 = DialogDefinitionFixture.CreateFirstDialogDefinition(dialogDefinition2, false);
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition1.Metadata.Id) || Equals(d.Metadata.Id, dialogDefinition2.Metadata.Id),
                                               dialog => Equals(dialog.Metadata.Id, dialogDefinition1.Metadata.Id)
                                                   ? DialogFixture.Create(dialogDefinition1.Metadata)
                                                   : DialogFixture.Create(dialogDefinition2.Metadata));
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>()))
                    .Returns<IDialogDefinitionIdentifier>(identifier =>
        {
            if (Equals(identifier.Id, dialogDefinition1.Metadata.Id) && Equals(identifier.Version, dialogDefinition1.Metadata.Version)) return Result<IDialogDefinition>.Success(dialogDefinition1);
            if (Equals(identifier.Id, dialogDefinition2.Metadata.Id) && Equals(identifier.Version, dialogDefinition2.Metadata.Version)) return Result<IDialogDefinition>.Success(dialogDefinition2);
            return Result<IDialogDefinition>.NotFound();
        });
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = await sut.Handle(new StartRequest(dialogDefinition1.Metadata), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.InProgress);
        result.Value!.CurrentDialogIdentifier.Id.Should().BeEquivalentTo(dialogDefinition2.Metadata.Id);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition2.Parts.First().Id);
    }

    [Fact]
    public async Task Handle_Uses_Result_From_NavigationPart()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateSingleStepDefinitionBuilder();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>()))
                    .Returns(Result<IDialogDefinition>.Success(dialogDefinition));
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = await sut.Handle(new StartRequest(dialogDefinition.Metadata), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.InProgress);
        result.Value!.CurrentPartId.Value.Should().Be("Welcome");
    }

    [Fact]
    public async Task Handle_Returns_Error_When_Dialog_Could_Not_Be_Created()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => throw new InvalidOperationException("Kaboom"));
        var provider = new TestDialogDefinitionProvider();
        var sut = new StartRequestHandler(factory, provider, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = await sut.Handle(new StartRequest(dialogDefinition.Metadata), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Dialog creation failed");
        AssertLogging("CreateDialogAndDefinition failed", "Kaboom");
    }

    [Fact]
    public async Task Handle_Returns_NotFound_When_DialogDefinition_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>()))
                    .Returns(Result<IDialogDefinition>.NotFound());
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = await sut.Handle(new StartRequest(dialogDefinition.Metadata), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        var msg = "Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]";
        result.ErrorMessage.Should().Be(msg);
        AssertLogging(msg, null);
    }

    [Fact]
    public async Task Handle_Returns_Error_When_DialogDefinition_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>()))
                    .Throws(new InvalidOperationException("Kaboom"));
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = await sut.Handle(new StartRequest(dialogDefinition.Metadata), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
    }

    [Fact]
    public async Task Handle_Returns_First_DialogPart_When_It_Could_Be_Determined()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new StartRequest(dialogDefinition.Metadata), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.Parts.OfType<IGroupedDialogPart>().First().Group.Id);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.Parts.First().Id);
    }

    [Fact]
    public async Task Handle_Returns_ErrorDialogPart_When_DecisionPart_Returns_ErrorDialogPart()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateDialogDefinitionWithDecisionPartThatReturnsErrorDialogPart();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>()))
                    .Returns(Result<IDialogDefinition>.Success(dialogDefinition));
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = await sut.Handle(new StartRequest(dialogDefinition.Metadata), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().NotBeNull();
        result.Value!.CurrentState.Should().Be(DialogState.ErrorOccured);
        result.Value!.CurrentPartId.Value.Should().Be("Error");
    }

    [Fact]
    public async Task Handle_Returns_AbortDialogPart_When_DecisionPart_Returns_AbortDialogPart()
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
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>()))
                    .Returns(Result<IDialogDefinition>.Success(dialogDefinition));
        var sut = new StartRequestHandler(factory, ProviderMock.Object, ConditionEvaluatorMock.Object, LoggerMock.Object);

        // Act
        var result = await sut.Handle(new StartRequest(dialogDefinition.Metadata), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.Aborted);
        result.Value!.CurrentPartId.Value.Should().Be("Abort");
    }

    [Fact]
    public async Task Handle_Fills_Results_From_Previous_Session_On_Dialog_When_Present()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = CreateSut();
        var partResult = new DialogPartResultBuilder()
            .WithDialogId(new DialogDefinitionIdentifierBuilder(dialogDefinition.Metadata))
            .WithDialogPartId(new DialogPartIdentifierBuilder(dialogDefinition.Parts.First().Id))
            .WithResultId(new DialogPartResultIdentifierBuilder())
            .Build();

        // Act
        var result = await sut.Handle(new StartRequest(dialogDefinition.Metadata, new[] { partResult }, Enumerable.Empty<IProperty>()), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().NotBeNull();
        result.GetValueOrThrow().GetAllResults(dialogDefinition).Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_Fills_Properties_From_Rquest_On_Dialog_When_Present()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = CreateSut();
        var properties = new[]
        {
            new PropertyBuilder().WithName("Test").WithValue("Value").Build()
        };

        // Act
        var result = await sut.Handle(new StartRequest(dialogDefinition.Metadata, Enumerable.Empty<IDialogPartResult>(), properties), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().NotBeNull();
        result.GetValueOrThrow().GetProperties().Should().ContainSingle();
    }

    private StartRequestHandler CreateSut()
        => new StartRequestHandler(new DialogFactory(),
                                   new TestDialogDefinitionProvider(),
                                   ConditionEvaluatorMock.Object,
                                   LoggerMock.Object);
}
