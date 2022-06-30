﻿namespace DialogFramework.Application.Tests.RequestHandlers;

public class ContinueRequestHandlerTests : RequestHandlerTestBase
{
    public ContinueRequestHandlerTests() : base() { }

    [Theory]
    [InlineData(DialogState.Aborted)]
    [InlineData(DialogState.Completed)]
    [InlineData(DialogState.ErrorOccured)]
    public void Handle_Returns_Invalid_On_Invalid_State(DialogState currentState)
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
        var result = sut.Handle(new ContinueRequest(dialog));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public void Handle_Returns_Next_DialogPart_When_Current_State_Is_Question_And_Answer_Is_Valid()
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
        var result = sut.Handle(new ContinueRequest(definition, new[] { dialogPartResult }));

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.Completed);
        result.Value!.CurrentPartId.Value.Should().Be("Completed");
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.CompletedPart.Group.Id);
    }

    [Fact]
    public void Handle_Returns_Invalid_When_Current_State_Is_Question_And_Answer_Is_Not_Valid()
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
        var result = sut.Handle(new ContinueRequest(dialog, new[] { dialogPartResult }));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Validation failed, see ValidationErrors for more details");
        result.ValidationErrors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(new[] { "Unknown Result Id: [DialogPartResultIdentifier { Value = Unknown result }]" });
    }

    [Fact]
    public void Handle_Returns_NotFound_When_Dialog_Could_Not_Be_Found()
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
        var result = sut.Handle(new ContinueRequest(dialog));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        var msg = "Unknown dialog definition: Id [DialogDefinitionFixture], Version [1.0.0]";
        result.ErrorMessage.Should().Be(msg);
        AssertLogging(msg, null);
    }

    [Fact]
    public void Handle_Returns_Error_When_Dialog_Retrieval_Throws()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var factory = new DialogFactoryFixture(d => Equals(d.Metadata.Id, dialogDefinition.Metadata.Id),
                                               _ => DialogFixture.Create(dialogDefinition.Metadata));
        ProviderMock.Setup(x => x.GetDialogDefinition(It.IsAny<IDialogDefinitionIdentifier>())).Throws(new InvalidOperationException("Kaboom"));
        var sut = CreateSut(factory);
        var dialog = factory.Create(dialogDefinition, Enumerable.Empty<IDialogPartResult>()).GetValueOrThrow();

        // Act
        var result = sut.Handle(new ContinueRequest(dialog));

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Could not retrieve dialog definition");
        AssertLogging("GetDialogDefinition failed", "Kaboom");
    }

    [Fact]
    public void Handle_Uses_Result_From_RedirectPart()
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
            .AddParts(welcomePart, redirectPart)
            .Build();
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
        var sut = CreateSut(factory);
        var dialog = DialogFixture.Create(dialogDefinition1.Metadata, welcomePart.Id.Build());

        // Act
        var result = sut.Handle(new ContinueRequest(dialog));

        // Assert
        result.IsSuccessful().Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value!.CurrentState.Should().Be(DialogState.InProgress);
        result.Value!.CurrentDialogIdentifier.Id.Should().BeEquivalentTo(dialogDefinition2.Metadata.Id);
        result.Value!.CurrentGroupId.Should().BeEquivalentTo(welcomePart.Group.Id.Build());
        result.Value!.CurrentPartId.Should().BeEquivalentTo(welcomePart.Id.Build());
    }

    private ContinueRequestHandler CreateSut(DialogFactoryFixture factory)
        => new ContinueRequestHandler(
            factory,
            ProviderMock.Object,
            ConditionEvaluatorMock.Object,
            LoggerMock.Object,
            new StartRequestHandler(
                factory,
                ProviderMock.Object,
                ConditionEvaluatorMock.Object,
                LoggerMock.Object));

    private ContinueRequestHandler CreateSut()
        => new ContinueRequestHandler(
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
