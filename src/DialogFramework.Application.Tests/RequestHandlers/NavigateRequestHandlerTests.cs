namespace DialogFramework.Application.Tests.RequestHandlers;

public class NavigateRequestHandlerTests : RequestHandlerTestBase
{
    public NavigateRequestHandlerTests() : base() { }

    [Fact]
    public async Task Handle_Returns_Invalid_When_Parts_Does_Not_Contain_Current_Part()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var completedPart = dialogDefinition.CompletedPart;
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new NavigateRequest(dialog, completedPart.Id), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Cannot navigate to the specified part");
    }

    [Fact]
    public async Task Handle_Returns_Invalid_When_Requested_Part_Is_Not_Part_Of_Current_Dialog()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var errorPart = dialogDefinition.ErrorPart;
        var completedPart = dialogDefinition.CompletedPart;
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, errorPart);
        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new NavigateRequest(dialog, completedPart.Id), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public async Task Handle_Returns_Invalid_When_Requested_Part_Is_After_Current_Part()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var messagePart = dialogDefinition.Parts.OfType<IMessageDialogPart>().First();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, messagePart);
        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new NavigateRequest(dialog, questionPart.Id), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Cannot navigate to the specified part");
    }

    [Fact]
    public async Task Handle_Returns_Success_When_Requested_Part_Is_Current_Part()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new NavigateRequest(dialog, questionPart.Id), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public async Task Handle_Returns_Success_When_Requested_Part_Is_Before_Current_Part()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var messagePart = dialogDefinition.Parts.OfType<IMessageDialogPart>().First();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, messagePart);
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(string.Empty))
            .Build();
        dialog.Continue(dialogDefinition, new[] { partResult }, ConditionEvaluatorMock.Object);
        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new NavigateRequest(dialog, messagePart.Id), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public async Task Handle_Navigates_To_Requested_Part_When_CanNavigate_Is_True()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build();
        var messagePart = dialogDefinition.Parts.OfType<IMessageDialogPart>().First();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, messagePart);
        var partResult = new DialogPartResultAnswerBuilder()
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(string.Empty))
            .Build();
        dialog.Continue(dialogDefinition, new[] { partResult }, ConditionEvaluatorMock.Object);
        var sut = CreateSut();

        // Act
        var result = await sut.Handle(new NavigateRequest(dialog, messagePart.Id), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.InProgress);
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(messagePart.Group.Id);
        result.Value!.CurrentPartId.Should().BeEquivalentTo(messagePart.Id);
    }

    [Fact]
    public async Task Handle_Returns_Error_When_DialogDefinition_Could_Not_Be_Found()
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
        var result = await sut.Handle(new NavigateRequest(dialog, dialogDefinition.Metadata, dialogDefinition.Parts.First().Id), CancellationToken.None);

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
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var sut = CreateSut(factory);
        var dialog = factory.Create(dialogDefinition, Enumerable.Empty<IDialogPartResult>()).GetValueOrThrow();

        // Act
        var result = await sut.Handle(new NavigateRequest(dialog, dialogDefinition.Parts.First().Id), CancellationToken.None);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
    }

    private NavigateRequestHandler CreateSut(DialogFactoryFixture factory)
        => new NavigateRequestHandler(
            factory,
            ProviderMock.Object,
            ConditionEvaluatorMock.Object,
            LoggerMock.Object,
            new StartRequestHandler(
                factory,
                ProviderMock.Object,
                ConditionEvaluatorMock.Object,
                LoggerMock.Object));

    private NavigateRequestHandler CreateSut()
        => new NavigateRequestHandler(
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
