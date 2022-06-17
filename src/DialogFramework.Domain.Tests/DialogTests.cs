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
        var result = sut.Abort(dialogDefinition);

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
        var result = sut.Abort(dialogDefinition);

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
        var result = sut.Abort(dialogDefinition);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        sut.CurrentPartId.Should().Be(dialogDefinition.AbortedPart.Id);
        sut.CurrentGroupId.Should().BeNull();
        sut.CurrentState.Should().Be(DialogState.Aborted);
    }

    [Fact]
    public void Continue_Returns_Invalid_When_CurrentState_Is_Not_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.CompletedPart);

        // Act
        var result = sut.Continue(dialogDefinition, Enumerable.Empty<IDialogPartResult>(), _conditionEvaluatorMock.Object);

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
        var sut = DialogFixture.Create
        (
            Id,
            dialogDefinition.Metadata,
            dialogDefinition.Parts.First(),
            new[]
            {
                new DialogPartResultBuilder()
                    .WithDialogPartId(new DialogPartIdentifierBuilder(dialogDefinition.Parts.First().Id))
                    .WithResultId(new DialogPartResultIdentifierBuilder(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().Results.First().Id))
                    .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(true))
                    .Build()
            }
        );

        // Act
        var result = sut.Continue
        (
            dialogDefinition,
            new[]
            {
                new DialogPartResultBuilder()
                    .WithDialogPartId(new DialogPartIdentifierBuilder(dialogDefinition.Parts.First().Id))
                    .WithResultId(new DialogPartResultIdentifierBuilder(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().Results.First().Id))
                    .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(false))
                    .Build()
            }, _conditionEvaluatorMock.Object);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        var nextPart = dialogDefinition.Parts.Skip(1).First();
        sut.CurrentPartId.Should().Be(nextPart.Id);
        sut.CurrentGroupId.Should().Be(nextPart.GetGroupId());
        sut.CurrentState.Should().Be(nextPart.GetState());
        //first result value (true) is replaced with second (false)
        sut.Results.Should().ContainSingle();
        sut.Results.Single().Value.Value.Should().BeEquivalentTo(false);
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
    public void NavigateTo_Returns_Invalid_When_CurrentState_Is_Not_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(dialogDefinition.Metadata); //state initial

        // Act
        var result = sut.NavigateTo(dialogDefinition, dialogDefinition.Parts.First().Id);

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
        var result = sut.NavigateTo(dialogDefinition, dialogDefinition.Parts.Skip(1).First().Id);

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
            .WithDialogPartId(partIdBuilder)
            .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("Something"))
            .Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.First(), new[] { partResult });

        // Act
        var result = sut.NavigateTo(dialogDefinition, partIdBuilder.Build());

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
                    .WithDialogPartId(new DialogPartIdentifierBuilder(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().Id))
                    .WithResultId(new DialogPartResultIdentifierBuilder(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().Results.First().Id))
                    .Build()
            }
        );

        // Act
        var result = sut.NavigateTo(dialogDefinition, dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().Id);

        // Assert
        result.IsSuccessful().Should().BeTrue();
        sut.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().Id);
        sut.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().GetGroupId());
        sut.CurrentState.Should().Be(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().GetState());
    }

    [Fact]
    public void ResetCurrentState_Returns_Invalid_When_CurrentState_Is_Not_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(dialogDefinition.Metadata);

        // Act
        var result = sut.ResetCurrentState(dialogDefinition);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ErrorMessage.Should().Be("Current state is invalid");
    }

    [Fact]
    public void ResetCurrentState_Returns_Invalid_When_Dialog_CanResetResultsByPartId_Is_False()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.OfType<IMessageDialogPart>().First());

        // Act
        var result = sut.ResetCurrentState(dialogDefinition);

        // Assert
        result.IsSuccessful().Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public void ResetCurrentState_Returns_Success_And_Updates_Results_Correctly_When_Validation_Succeeds()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        var dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        dialog.Start(dialogDefinition, _conditionEvaluatorMock.Object);
        dialog.Continue(dialogDefinition, new[] { new DialogPartResult(questionPart.Id, questionPart.Results.First().Id, new YesNoDialogPartResultValue(true)) }, conditionEvaluatorMock.Object);
        dialog.NavigateTo(dialogDefinition, questionPart.Id);
        dialog.GetDialogPartResultsByPartIdentifier(questionPart.Id).Should().ContainSingle();
        dialog.GetDialogPartResultsByPartIdentifier(questionPart.Id).Single().ResultId.Should().Be(questionPart.Results.First().Id);

        // Act 1 - Call reset while there is an answer
        dialog.ResetCurrentState(dialogDefinition);
        // Assert 1
        dialog.GetDialogPartResultsByPartIdentifier(questionPart.Id).Should().BeEmpty();

        // Act 2 - Call reset while there is no answer
        dialog.ResetCurrentState(dialogDefinition);
        // Assert 2
        dialog.GetDialogPartResultsByPartIdentifier(questionPart.Id).Should().BeEmpty();
    }

    [Fact]
    public void ResetCurrentState_Returns_Success_When_Validation_Succeeds()
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
        var result = sut.ResetCurrentState(dialogDefinition);

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
        var result = sut.Error(dialogDefinition, new Error("message"));

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
        var result = sut.Error(dialogDefinition, null);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.ErrorMessage.Should().BeNull();
        result.IsSuccessful().Should().BeTrue();
        sut.CurrentPartId.Should().Be(dialogDefinition.ErrorPart.Id);
        sut.ErrorMessage.Should().BeNull();
    }
}
