namespace DialogFramework.Domain.Tests;

public class DialogTests
{
    private static string Id => Guid.NewGuid().ToString();
    private readonly Mock<IConditionEvaluator> _conditionEvaluatorMock;

    public DialogTests()
    {
        _conditionEvaluatorMock = new Mock<IConditionEvaluator>();
    }

    [Fact]
    public void Abort_Returns_Invalid_When_CurrentPart_Is_AbortedPart()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.AbortedPart);

        // Act
        var result = sut.Abort(dialogDefinition, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Dialog has already been aborted");
    }

    [Fact]
    public void Abort_Returns_Invalid_When_CurrentState_Is_Not_InProgess()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.CompletedPart);

        // Act
        var result = sut.Abort(dialogDefinition, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public void Abort_Returns_Success_And_Updates_State_Correcly_When_Validation_Succeeds()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.First());

        // Act
        var result = sut.Abort(dialogDefinition, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        sut.CurrentPartId.Should().Be(dialogDefinition.AbortedPart.Id);
        sut.CurrentGroupId.Should().BeNull();
        sut.CurrentState.Should().Be(DialogState.Aborted);
    }

    [Fact]
    public void Abort_Calls_AfterNavigate_On_Previous_Part()
    {
        // Arrange
        var data = DialogDefinitionFixture.CreateNavigatableDialogDefinition();
        var sut = DialogFixture.Create(Id, data.DialogMock.Object.Metadata, data.MessagePartMock.Object);

        // Act
        _ = sut.Abort(data.DialogMock.Object, _conditionEvaluatorMock.Object);

        // Assert
        data.MessagePartMock.Verify(x => x.AfterNavigate(It.IsAny<IAfterNavigateArguments>()), Times.Once);
    }

    [Fact]
    public void Abort_Calls_BeforeNavigate_On_Current_Part()
    {
        // Arrange
        var data = DialogDefinitionFixture.CreateNavigatableDialogDefinition();
        var sut = DialogFixture.Create(Id, data.DialogMock.Object.Metadata, data.MessagePartMock.Object);

        // Act
        _ = sut.Abort(data.DialogMock.Object, _conditionEvaluatorMock.Object);

        // Assert
        data.AbortPartMock.Verify(x => x.BeforeNavigate(It.IsAny<IBeforeNavigateArguments>()), Times.Once);
    }

    [Fact]
    public void Continue_Returns_Invalid_When_CurrentState_Is_Not_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.CompletedPart);

        // Act
        var result = sut.Continue(dialogDefinition, Enumerable.Empty<IDialogPartResultAnswer>(), _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public void Continue_Returns_Success_And_Updates_State_Correctly_When_Validation_Succeeds()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().First();
        var sut = DialogFixture.Create
        (
            Id,
            dialogDefinition.Metadata,
            dialogDefinition.Parts.First(),
            new[] { CreatePartResult(dialogDefinition, questionPart) }
        );

        // Act
        var result = sut.Continue
        (
            dialogDefinition,
            new[]
            {
                new DialogPartResultAnswerBuilder()
                    .WithResultId(new DialogPartResultIdentifierBuilder(questionPart.Results.First().Id))
                    .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(false))
                    .Build()
            }, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        var nextPart = dialogDefinition.Parts.Skip(1).First();
        sut.CurrentPartId.Should().Be(nextPart.Id);
        sut.CurrentGroupId.Should().Be(nextPart.GetGroupId());
        sut.CurrentState.Should().Be(nextPart.GetState());
        //first result value (true) is replaced with second (false)
        var results = sut.GetDialogPartResultsByPartIdentifier(questionPart.Id).GetValueOrThrow();
        results.Should().ContainSingle();
        results.Single().Value.Value.Should().BeEquivalentTo(false);
    }

    [Fact]
    public void Continue_Calls_AfterNavigate_On_Previous_Part()
    {
        // Arrange
        var data = DialogDefinitionFixture.CreateNavigatableDialogDefinition();
        var sut = DialogFixture.Create(Id, data.DialogMock.Object.Metadata, data.MessagePartMock.Object);

        // Act
        _ = sut.Continue(data.DialogMock.Object, Enumerable.Empty<IDialogPartResultAnswer>(), _conditionEvaluatorMock.Object);

        // Assert
        data.MessagePartMock.Verify(x => x.AfterNavigate(It.IsAny<IAfterNavigateArguments>()), Times.Once);
    }

    [Fact]
    public void Continue_Calls_BeforeNavigate_On_Current_Part()
    {
        // Arrange
        var data = DialogDefinitionFixture.CreateNavigatableDialogDefinition();
        var sut = DialogFixture.Create(Id, data.DialogMock.Object.Metadata, data.MessagePartMock.Object);

        // Act
        _ = sut.Continue(data.DialogMock.Object, Enumerable.Empty<IDialogPartResultAnswer>(), _conditionEvaluatorMock.Object);

        // Assert
        data.CompletedPartMock.Verify(x => x.BeforeNavigate(It.IsAny<IBeforeNavigateArguments>()), Times.Once);
    }

    [Fact]
    public void Start_Returns_Invalid_When_CurrentState_Is_Not_Initial()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.First());

        // Act
        var result = sut.Start(dialogDefinition, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public void Start_Returns_Invalid_When_Metadata_CanStart_Is_False()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture
            .CreateBuilder()
            .WithMetadata(new DialogMetadataBuilder()
                .WithCanStart(false)
                .WithId("Id"))
            .Build();
        var sut = DialogFixture.Create(dialogDefinition.Metadata);

        // Act
        var result = sut.Start(dialogDefinition, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Dialog definition cannot be started");
    }

    [Fact]
    public void Start_Returns_Error_When_First_Part_Is_Dynamic_And_Gives_Error()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture
            .CreateBuilderBase()
            .AddParts(new NavigationDialogPartBuilder()
                .WithNavigateToId(new DialogPartIdentifierBuilder().WithValue("Non existing id"))
                .WithId(new DialogPartIdentifierBuilder()))
            .WithMetadata(new DialogMetadataBuilder()
                .WithId("Id"))
            .Build();
        var sut = DialogFixture.Create(dialogDefinition.Metadata);

        // Act
        var result = sut.Start(dialogDefinition, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Dialog does not have a part with id [DialogPartIdentifier { Value = Non existing id }]");
    }

    [Fact]
    public void Start_Returns_Success_And_Updates_State_Correctly_When_Validations_Succeeed()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(dialogDefinition.Metadata);

        // Act
        var result = sut.Start(dialogDefinition, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        sut.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.Parts.First().Id);
        sut.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.Parts.First().GetGroupId());
        sut.CurrentState.Should().Be(dialogDefinition.Parts.First().GetState());
    }

    [Fact]
    public void Start_Calls_BeforeNavigate_On_Current_Part()
    {
        // Arrange
        var data = DialogDefinitionFixture.CreateNavigatableDialogDefinition();
        var sut = DialogFixture.Create(data.DialogMock.Object.Metadata);

        // Act
        _ = sut.Start(data.DialogMock.Object, _conditionEvaluatorMock.Object);

        // Assert
        data.MessagePartMock.Verify(x => x.BeforeNavigate(It.IsAny<IBeforeNavigateArguments>()), Times.Once);
    }

    [Fact]
    public void NavigateTo_Returns_Invalid_When_CurrentState_Is_Not_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(dialogDefinition.Metadata); //state initial

        // Act
        var result = sut.NavigateTo(dialogDefinition, dialogDefinition.Parts.First().Id, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public void NavigateTo_Returns_Invalid_When_Dialog_CanNavigateTo_Is_False()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.First());

        // Act
        var result = sut.NavigateTo(dialogDefinition, dialogDefinition.Parts.Skip(1).First().Id, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public void NavigateTo_Returns_NotFound_When_Requested_Part_Does_Not_Exist_On_Dialog()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var partIdBuilder = new DialogPartIdentifierBuilder().WithValue("NonExisting");
        var partResult = new DialogPartResultBuilder()
            .WithDialogId(new DialogDefinitionIdentifierBuilder(dialogDefinition.Metadata))
            .WithDialogPartId(partIdBuilder)
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Something"))
            .Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.First(), new[] { partResult });

        // Act
        var result = sut.NavigateTo(dialogDefinition, partIdBuilder.Build(), _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public void NavigateTo_Returns_Success_And_Updates_State_Correctly_When_Validation_Succeeds()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create
        (
            Id,
            dialogDefinition.Metadata,
            dialogDefinition.Parts.Skip(1).First(),
            new[]
            {
                new DialogPartResultBuilder()
                    .WithDialogId(new DialogDefinitionIdentifierBuilder(dialogDefinition.Metadata))
                    .WithDialogPartId(new DialogPartIdentifierBuilder(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().Id))
                    .WithResultId(new DialogPartResultIdentifierBuilder(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().Results.First().Id))
                    .Build()
            }
        );

        // Act
        var result = sut.NavigateTo(dialogDefinition, dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().Id, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        sut.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().Id);
        sut.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().GetGroupId());
        sut.CurrentState.Should().Be(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().GetState());
    }

    [Fact]
    public void NavigateTo_Calls_AfterNavigate_On_Previous_Part()
    {
        // Arrange
        var data = DialogDefinitionFixture.CreateNavigatableDialogDefinition();
        var sut = DialogFixture.Create(Id, data.DialogMock.Object.Metadata, data.MessagePartMock.Object);

        // Act
        _ = sut.NavigateTo(data.DialogMock.Object, data.MessagePartMock.Object.Id, _conditionEvaluatorMock.Object);

        // Assert
        data.MessagePartMock.Verify(x => x.AfterNavigate(It.IsAny<IAfterNavigateArguments>()), Times.Once);
    }

    [Fact]
    public void NavigateTo_Calls_BeforeNavigate_On_Current_Part()
    {
        // Arrange
        var data = DialogDefinitionFixture.CreateNavigatableDialogDefinition();
        var sut = DialogFixture.Create(Id, data.DialogMock.Object.Metadata, data.MessagePartMock.Object);

        // Act
        _ = sut.NavigateTo(data.DialogMock.Object, data.MessagePartMock.Object.Id, _conditionEvaluatorMock.Object);

        // Assert
        data.MessagePartMock.Verify(x => x.BeforeNavigate(It.IsAny<IBeforeNavigateArguments>()), Times.Once);
    }

    [Fact]
    public void ResetState_Returns_Invalid_When_CurrentState_Is_Not_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(dialogDefinition.Metadata);

        // Act
        var result = sut.ResetState(dialogDefinition, sut.CurrentPartId);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public void ResetState_Returns_Invalid_When_Dialog_CanResetResultsByPartId_Is_False()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.OfType<IMessageDialogPart>().First());

        // Act
        var result = sut.ResetState(dialogDefinition, sut.CurrentPartId);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public void ResetState_Returns_Success_And_Updates_Results_Correctly_When_Validation_Succeeds()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        dialog.Start(dialogDefinition, _conditionEvaluatorMock.Object);
        dialog.Continue(dialogDefinition, new[] { new DialogPartResultAnswer(questionPart.Results.First().Id, new DialogPartResultValueAnswer(true)) }, conditionEvaluatorMock.Object);
        dialog.NavigateTo(dialogDefinition, questionPart.Id, _conditionEvaluatorMock.Object);
        var results = dialog.GetDialogPartResultsByPartIdentifier(questionPart.Id).GetValueOrThrow();
        results.Should().ContainSingle();
        results.Single().ResultId.Should().Be(questionPart.Results.First().Id);

        // Act 1 - Call reset while there is an answer
        dialog.ResetState(dialogDefinition, dialog.CurrentPartId);
        // Assert 1
        dialog.GetDialogPartResultsByPartIdentifier(questionPart.Id).GetValueOrThrow().Should().BeEmpty();

        // Act 2 - Call reset while there is no answer
        dialog.ResetState(dialogDefinition, dialog.CurrentPartId);
        // Assert 2
        dialog.GetDialogPartResultsByPartIdentifier(questionPart.Id).GetValueOrThrow().Should().BeEmpty();
    }

    [Fact]
    public void ResetState_Returns_Success_When_Validation_Succeeds()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create
        (
            Id,
            dialogDefinition.Metadata,
            dialogDefinition.Parts.OfType<IQuestionDialogPart>().First(),
            Enumerable.Empty<IDialogPartResult>()
        );

        // Act
        var result = sut.ResetState(dialogDefinition, sut.CurrentPartId);

        // Assert
        result.IsSuccessful().Should().BeTrue();
    }

    [Fact]
    public void Error_Returns_Ok_And_Updates_State_Correctly_With_ErrorMessage()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(dialogDefinition.Metadata); //state initial

        // Act
        var result = sut.Error(dialogDefinition, _conditionEvaluatorMock.Object, new Error("message"));

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.ErrorMessage.Should().BeNull();
        result.IsSuccessful().Should().BeTrue();
        sut.CurrentPartId.Should().Be(dialogDefinition.ErrorPart.Id);
        sut.ErrorMessage.Should().Be("message");
    }

    [Fact]
    public void Error_Returns_Ok_And_Updates_State_Correctly_Without_ErrorMessage()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(dialogDefinition.Metadata); //state initial

        // Act
        var result = sut.Error(dialogDefinition, _conditionEvaluatorMock.Object, null);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.ErrorMessage.Should().BeNull();
        result.IsSuccessful().Should().BeTrue();
        sut.CurrentPartId.Should().Be(dialogDefinition.ErrorPart.Id);
        sut.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Error_Calls_AfterNavigate_On_Previous_Part()
    {
        // Arrange
        var data = DialogDefinitionFixture.CreateNavigatableDialogDefinition();
        var sut = DialogFixture.Create(Id, data.DialogMock.Object.Metadata, data.MessagePartMock.Object);

        // Act
        _ = sut.Error(data.DialogMock.Object, _conditionEvaluatorMock.Object, default);

        // Assert
        data.MessagePartMock.Verify(x => x.AfterNavigate(It.IsAny<IAfterNavigateArguments>()), Times.Once);
    }

    [Fact]
    public void Error_Calls_BeforeNavigate_On_Current_Part()
    {
        // Arrange
        var data = DialogDefinitionFixture.CreateNavigatableDialogDefinition();
        var sut = DialogFixture.Create(Id, data.DialogMock.Object.Metadata, data.MessagePartMock.Object);

        // Act
        _ = sut.Error(data.DialogMock.Object, _conditionEvaluatorMock.Object, default);

        // Assert
        data.ErrorPartMock.Verify(x => x.BeforeNavigate(It.IsAny<IBeforeNavigateArguments>()), Times.Once);
    }

    [Fact]
    public void Can_Add_Properties_From_DialogPart()
    {
        // Arrange
        var afterNavigateCallback = new Action<IAfterNavigateArguments>(args => { });
        var beforeNavigateCallback = new Action<IBeforeNavigateArguments>(args => args.AddProperty(new Property("Added", "Value")));
        var definition = DialogDefinitionFixture.CreateBuilderBase()
            .AddParts(DialogPartFixture.CreateAddPropertiesDialogPartBuilder(afterNavigateCallback, beforeNavigateCallback))
            .Build();
        var dialog = DialogFixture.Create(definition.Metadata);

        // Act
        _ = dialog.Start(definition, _conditionEvaluatorMock.Object);

        // Assert
        dialog.GetProperties().Should().ContainSingle();
    }

    [Fact]
    public void Can_Set_Result_From_DialogPart()
    {
        // Arrange
        var afterNavigateCallback = new Action<IAfterNavigateArguments>(args => { });
        var beforeNavigateCallback = new Action<IBeforeNavigateArguments>(args => args.Result = Result.Error());
        var definition = DialogDefinitionFixture.CreateBuilderBase()
            .AddParts(DialogPartFixture.CreateAddPropertiesDialogPartBuilder(afterNavigateCallback, beforeNavigateCallback))
            .Build();
        var dialog = DialogFixture.Create(definition.Metadata);

        // Act
        var result = dialog.Start(definition, _conditionEvaluatorMock.Object);

        // Assert
        result.Should().BeEquivalentTo(Result.Error());
    }

    [Fact]
    public void Can_Cancel_Update_Of_State_From_DialogPart()
    {
        // Arrange
        var afterNavigateCallback = new Action<IAfterNavigateArguments>(args => { });
        var beforeNavigateCallback = new Action<IBeforeNavigateArguments>(args => args.CancelStateUpdate());
        var definition = DialogDefinitionFixture.CreateBuilderBase()
            .AddParts(DialogPartFixture.CreateAddPropertiesDialogPartBuilder(afterNavigateCallback, beforeNavigateCallback))
            .Build();
        var dialog = DialogFixture.Create(definition.Metadata);

        // Act
        var result = dialog.Start(definition, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        dialog.CurrentState.Should().Be(DialogState.Initial);
    }

    [Fact]
    public void Can_Update_State_With_Custom_Values_From_DialogPart()
    {
        // Arrange
        var afterNavigateCallback = new Action<IAfterNavigateArguments>(args => { });
        var beforeNavigateCallback = new Action<IBeforeNavigateArguments>(args =>
        {
            // First, Cancel the default update
            args.CancelStateUpdate();

            // Then, update the state with custom values
            args.CurrentState = DialogState.Aborted;
            args.CurrentDialogIdentifier = new DialogDefinitionIdentifier("CustomDialog", "2.0.0");
            args.CurrentGroupId = new DialogPartGroupIdentifier("CustomGroup");
            args.CurrentPartId = new DialogPartIdentifier("CustomPart");
            args.ErrorMessage = "Custom error message";
        });
        var definition = DialogDefinitionFixture.CreateBuilderBase()
            .AddParts(DialogPartFixture.CreateAddPropertiesDialogPartBuilder(afterNavigateCallback, beforeNavigateCallback))
            .Build();
        var dialog = DialogFixture.Create(definition.Metadata);

        // Act
        var result = dialog.Start(definition, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        dialog.CurrentState.Should().Be(DialogState.Aborted);
        dialog.CurrentDialogIdentifier.Should().BeEquivalentTo(new DialogDefinitionIdentifier("CustomDialog", "2.0.0"));
        dialog.CurrentGroupId.Should().BeEquivalentTo(new DialogPartGroupIdentifier("CustomGroup"));
        dialog.CurrentPartId.Should().BeEquivalentTo(new DialogPartIdentifier("CustomPart"));
        dialog.ErrorMessage.Should().Be("Custom error message");
    }

    private static IDialogPartResult CreatePartResult(IDialogDefinition dialogDefinition, IQuestionDialogPart questionPart)
        => new DialogPartResultBuilder()
            .WithDialogId(new DialogDefinitionIdentifierBuilder(dialogDefinition.Metadata))
            .WithDialogPartId(new DialogPartIdentifierBuilder(questionPart.Id))
            .WithResultId(new DialogPartResultIdentifierBuilder(questionPart.Results.First().Id))
            .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(true))
            .Build();
}
