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
    public void CanAbort_Returns_False_When_CurrentPart_Is_AbortedPart()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.AbortedPart);

        // Act
        var actual = sut.CanAbort(dialogDefinition);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void CanAbort_Returns_False_When_CurrentState_Is_Not_InProgess()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.CompletedPart);

        // Act
        var actual = sut.CanAbort(dialogDefinition);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void CanAbort_Returns_True_When_CurrentState_Is_InProgess_And_CurrentPart_Is_Not_AbortedPart()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.First());

        // Act
        var actual = sut.CanAbort(dialogDefinition);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void Abort_Throws_When_CanAbort_Is_False()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.CompletedPart);

        // Act
        var act = new Action(() => sut.Abort(dialogDefinition));

        // Assert
        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void Abort_Updates_State_Correcly_When_CanAbort_Is_True()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.First());

        // Act
        sut.Abort(dialogDefinition);

        // Assert
        sut.CurrentPartId.Should().Be(dialogDefinition.AbortedPart.Id);
        sut.CurrentGroupId.Should().BeNull();
        sut.CurrentState.Should().Be(DialogState.Aborted);
    }

    [Fact]
    public void CanContinue_Returns_False_When_CurrentState_Is_Not_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.CompletedPart);

        // Act
        var actual = sut.CanContinue(dialogDefinition, Enumerable.Empty<IDialogPartResult>());

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void CanContinue_Returns_True_When_CurrentState_Is_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.First());

        // Act
        var actual = sut.CanContinue(dialogDefinition, Enumerable.Empty<IDialogPartResult>());

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void Continue_Throws_When_CanContinue_Is_False()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.CompletedPart);

        // Act
        var act = new Action(() => sut.Continue(dialogDefinition, Enumerable.Empty<IDialogPartResult>(), _conditionEvaluatorMock.Object));

        // Assert
        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void Continue_Updates_State_Correctly_When_CanContinue_Is_True()
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
            },
            new[]
            {
                new DialogValidationResultBuilder()
                    .WithErrorMessage("You fool! You provided the wrong input")
                    .Build()
            }
        );

        // Act
        sut.Continue
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
        var nextPart = dialogDefinition.Parts.Skip(1).First();
        sut.CurrentPartId.Should().Be(nextPart.Id);
        sut.CurrentGroupId.Should().Be(nextPart.GetGroupId());
        sut.CurrentState.Should().Be(nextPart.GetState());
        sut.ValidationErrors.Should().BeEmpty(); //cleared
        //first result value (true) is replaced with second (false)
        sut.Results.Should().ContainSingle();
        sut.Results.Single().Value.Value.Should().BeEquivalentTo(false);
    }

    [Fact]
    public void Error_Updates_State_Correctly()
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
            },
            new[]
            {
                new DialogValidationResultBuilder()
                    .WithErrorMessage("You fool! You provided the wrong input")
                    .Build()
            },
            new[]
            {
                new ErrorBuilder().WithMessage("Kaboom").Build()
            }
        );
        // Act
        sut.Error(dialogDefinition, new Error("Something went wrong"));

        // Assert
        sut.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.ErrorPart.Id);
        sut.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.ErrorPart.GetGroupId());
        sut.CurrentState.Should().Be(DialogState.ErrorOccured);
        sut.Errors.Should().BeEquivalentTo(new[] { new Error("Something went wrong") });
        sut.ValidationErrors.Should().BeEmpty(); // cleared
        sut.Results.Should().ContainSingle(); // not overwritten
    }

    [Fact]
    public void CanStart_Returns_True_When_CurrentState_Is_Initial_And_Metadata_CanStart_Is_True()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(dialogDefinition.Metadata);

        // Act
        var actual = sut.CanStart(dialogDefinition, _conditionEvaluatorMock.Object);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void CanStart_Returns_False_When_CurrentState_Is_Not_Initial()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.First());

        // Act
        var actual = sut.CanStart(dialogDefinition, _conditionEvaluatorMock.Object);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void CanStart_Returns_False_When_Metadata_CanStart_Is_False()
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
        var actual = sut.CanStart(dialogDefinition, _conditionEvaluatorMock.Object);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void Start_Throws_When_CanStart_Is_False()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.First());

        // Act
        var act = new Action(() => sut.Start(dialogDefinition, _conditionEvaluatorMock.Object));

        // Assert
        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void Start_Updates_State_Correctly_When_CanStart_Is_True()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(dialogDefinition.Metadata);

        // Act
        sut.Start(dialogDefinition, _conditionEvaluatorMock.Object);

        // Assert
        sut.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.Parts.First().Id);
        sut.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.Parts.First().GetGroupId());
        sut.CurrentState.Should().Be(dialogDefinition.Parts.First().GetState());
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_CurrentState_Is_Not_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(dialogDefinition.Metadata); //state initial

        // Act
        var actual = sut.CanNavigateTo(dialogDefinition, dialogDefinition.Parts.First().Id);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_False_When_Dialog_CanNavigateTo_Is_False()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.First());

        // Act
        var actual = sut.CanNavigateTo(dialogDefinition, dialogDefinition.Parts.Skip(1).First().Id);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void CanNavigateTo_Returns_True_When_CurrentState_Is_InProgress_And_Dialog_CanNavigateTo_Is_True()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.First());

        // Act
        var actual = sut.CanNavigateTo(dialogDefinition, dialogDefinition.Parts.First().Id);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void NavigateTo_Throws_When_CanNavigateTo_Is_False()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.First());

        // Act
        var act = new Action(() => sut.NavigateTo(dialogDefinition, dialogDefinition.Parts.Skip(1).First().Id));

        // Assert
        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void NavigateTo_Updates_State_Correctly_When_CanNavigateTo_Is_True()
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
            },
            new[]
            {
                new DialogValidationResultBuilder().WithErrorMessage("Validation error").Build()
            }
        );

        // Act
        sut.NavigateTo(dialogDefinition, dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().Id);

        // Assert
        sut.CurrentPartId.Should().BeEquivalentTo(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().Id);
        sut.CurrentGroupId.Should().BeEquivalentTo(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().GetGroupId());
        sut.CurrentState.Should().Be(dialogDefinition.Parts.OfType<IQuestionDialogPart>().First().GetState());
        sut.ValidationErrors.Should().BeEmpty();
    }

    [Fact]
    public void CanResetCurrentState_Returns_False_When_CurrentState_Is_Not_InProgress()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(dialogDefinition.Metadata);

        // Act
        var actual = sut.CanResetCurrentState(dialogDefinition);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void CanResetCurrentState_Returns_False_When_Dialog_CanResetResultsByPartId_Is_False()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.OfType<IMessageDialogPart>().First());

        // Act
        var actual = sut.CanResetCurrentState(dialogDefinition);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void CanResetCurrentState_Returns_True_When_CurrentState_Is_InProgress_And_Dialog_CanResetResultsByPartId_Is_True()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.OfType<IQuestionDialogPart>().First());

        // Act
        var actual = sut.CanResetCurrentState(dialogDefinition);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void ResetCurrentState_Throws_When_CanResetCurrentState_Is_False()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create(Id, dialogDefinition.Metadata, dialogDefinition.Parts.OfType<IMessageDialogPart>().First());

        // Act
        var act = new Action(() => sut.ResetCurrentState(dialogDefinition));

        // Assert
        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void ResetCurrentState_Updates_Results_Correctly_When_CanResetCurrentState_Is_True()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var questionPart = dialogDefinition.Parts.OfType<IQuestionDialogPart>().Single();
        var conditionEvaluatorMock = new Mock<IConditionEvaluator>();
        IDialog dialog = DialogFixture.Create(Id, dialogDefinition.Metadata, questionPart);
        dialog.Continue(dialogDefinition, new[] { new DialogPartResult(questionPart.Id, questionPart.Results.First().Id, new EmptyDialogPartResultValue()) }, conditionEvaluatorMock.Object);
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
    public void ResetCurrentState_Clears_ValidationErrors_When_CanResetCurrentState_Is_True()
    {
        // Arrange
        var dialogDefinition = DialogDefinitionFixture.CreateBuilder().Build();
        var sut = DialogFixture.Create
        (
            Id,
            dialogDefinition.Metadata,
            dialogDefinition.Parts.OfType<IQuestionDialogPart>().First(),
            Enumerable.Empty<IDialogPartResult>(),
            new[]
            {
                new DialogValidationResultBuilder().WithErrorMessage("Validation error").Build()
            }
        );

        // Act
        sut.ResetCurrentState(dialogDefinition);

        // Assert
        sut.ValidationErrors.Should().BeEmpty();
    }
}
