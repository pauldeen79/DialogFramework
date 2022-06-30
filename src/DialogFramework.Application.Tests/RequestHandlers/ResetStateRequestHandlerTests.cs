namespace DialogFramework.Application.Tests.RequestHandlers;

public class ResetStateRequestHandlerTests : RequestHandlerTestBase
{
    public ResetStateRequestHandlerTests() : base() { }

    [Fact]
    public void Handle_Returns_Invalid_When_CurrentState_Is_Not_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.AbortedPart);
        var sut = CreateSut();

        // Act
        var result = sut.Handle(new ResetStateRequest(dialog));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public void Handle_Returns_Invalid_When_Current_DialogPart_Is_Not_QuestionDialogPart()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.CompletedPart); // note that this actually invalid state, but we currently can't prevent it on the interface
        var sut = CreateSut();

        // Act
        var result = sut.Handle(new ResetStateRequest(dialog));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public void Handle_Resets_Answers_From_Current_Question_When_Possible()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        dialog = DialogFixture.Create(dialog, dialogDefinition, new[]
        {
            new DialogPartResultBuilder()
                .WithDialogId(new DialogDefinitionIdentifierBuilder(dialogDefinition.Metadata))
                .WithDialogPartId(new DialogPartIdentifierBuilder(questionPart.Id))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Terrible"))
                .Build(),
            new DialogPartResultBuilder()
                .WithDialogId(new DialogDefinitionIdentifierBuilder(dialogDefinition.Metadata))
                .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue("Other part"))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Other value"))
                .Build()
        });
        var sut = CreateSut();

        // Act
        var result = sut.Handle(new ResetStateRequest(dialog));

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        var dialogPartResults = dialog.GetDialogPartResultsByPartIdentifier(questionPart.Id).GetValueOrThrow();
        dialogPartResults.Should().BeEmpty();
    }

    [Fact]
    public void Handle_Returns_Error_When_DialogDefinition_Could_Not_Be_Found()
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
        var result = sut.Handle(new ResetStateRequest(dialog));

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
        var sut = CreateSut(factory);
        var dialog = factory.Create(dialogDefinition, Enumerable.Empty<IDialogPartResult>()).GetValueOrThrow();

        // Act
        var result = sut.Handle(new ResetStateRequest(dialog));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
    }

    private ResetStateRequestHandler CreateSut(DialogFactoryFixture factory)
        => new ResetStateRequestHandler(
            factory,
            ProviderMock.Object,
            ConditionEvaluatorMock.Object,
            LoggerMock.Object,
            new StartRequestHandler(
                factory,
                ProviderMock.Object,
                ConditionEvaluatorMock.Object,
                LoggerMock.Object));

    private ResetStateRequestHandler CreateSut()
        => new ResetStateRequestHandler(
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
