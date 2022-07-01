namespace DialogFramework.Application.Tests.RequestHandlers;

public class AbortRequestHandlerTests : RequestHandlerTestBase
{
    public AbortRequestHandlerTests() : base() { }

    [Fact]
    public async Task Handle_Returns_Invalid_When_Validation_Fails()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var abortedPart = dialogDefinition.AbortedPart;
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, abortedPart);
        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new AbortRequest(dialog), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_Returns_AbortDialogPart_Dialog_When_Validation_Succeeds()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var abortedPart = dialogDefinition.AbortedPart;
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new AbortRequest(dialog), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.Aborted);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(abortedPart.Id);
    }

    [Fact]
    public async Task Handle_Returns_NotFound_When_Dialog_Could_Not_Be_Found()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>()))
                     .Returns(Result<IDialogDefinition>.NotFound());
        var sut = CreateSut(factory);
        var dialog = factory.Create(dialogDefinition, Enumerable.Empty<IDialogPartResult>()).GetValueOrThrow();

        // Act
        var result = await sut.Handle(new AbortRequest(dialog), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        var msg = "Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]";
        result.ErrorMessage.Should().Be(msg);
        AssertLogging(msg, null);
    }

    [Fact]
    public async Task Handle_Returns_Error_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var sut = CreateSut(factory);
        var dialog = factory.Create(dialogDefinition, Enumerable.Empty<IDialogPartResult>()).GetValueOrThrow();

        // Act
        var result = await sut.Handle(new AbortRequest(dialog), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
    }

    private AbortRequestHandler CreateSut(DialogFactoryFixture factory)
        => new AbortRequestHandler(
            factory,
            ProviderMock.Object,
            ConditionEvaluatorMock.Object,
            LoggerMock.Object,
            new StartRequestHandler(
                factory,
                ProviderMock.Object,
                ConditionEvaluatorMock.Object,
                LoggerMock.Object));

    private AbortRequestHandler CreateSut()
        => new AbortRequestHandler(
            new DialogFactory(),
            new TestDialogDefinitionProvider(),
            ConditionEvaluatorMock.Object,
            LoggerMock.Object,
            new StartRequestHandler(
                new DialogFactory(),
                new TestDialogDefinitionProvider(),
                ConditionEvaluatorMock.Object,
                LoggerMock.Object));
}
