namespace DialogFramework.Application.Tests;

public sealed class DialogServiceTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;
    private readonly IDialogSubmitter _dialogSubmitterMock;
    private readonly IDialogRepository _dialogRepositoryMock;

    public DialogServiceTests()
    {
        _dialogSubmitterMock = Substitute.For<IDialogSubmitter>();
        _dialogRepositoryMock = Substitute.For<IDialogRepository>();
        _provider = new ServiceCollection()
            .AddDialogFramework()
            .AddScoped(_ => _dialogSubmitterMock)
            .AddScoped(_ => _dialogRepositoryMock)
            .BuildServiceProvider(true);

        _scope = _provider.CreateScope();
    }

    [Fact]
    public void Submit_Returns_NotSupported_When_DialogDefinition_Has_No_Submitter()
    {
        // Arrange
        var dialog = TestDialogFactory.CreateEmpty();
        var definition = TestDialogDefinitionFactory.CreateEmpty();
        _dialogSubmitterMock.SupportsDialog(dialog.DefinitionId, dialog.DefinitionVersion).Returns(false);
        _dialogRepositoryMock.Get(dialog.DefinitionId, dialog.DefinitionVersion).Returns(Result.Success(definition));

        // Act
        var result = CreateSut().Submit(dialog);

        // Assert
        result.Status.Should().Be(ResultStatus.NotSupported);
    }

    [Fact]
    public void Submit_Returns_Result_Of_Last_Submitter_When_Multiple_Submitters_Are_Found_For_Specified_Dialog()
    {
        // Arrange
        var dialog = TestDialogFactory.CreateEmpty();
        var definition = TestDialogDefinitionFactory.CreateEmpty();
        var resultDialog = TestDialogFactory.CreateEmpty();
        _dialogSubmitterMock.SupportsDialog(dialog.DefinitionId, dialog.DefinitionVersion).Returns(true);
        _dialogSubmitterMock.Submit(Arg.Any<Dialog>()).Returns(Result.Success(resultDialog));
        _dialogRepositoryMock.Get(dialog.DefinitionId, dialog.DefinitionVersion).Returns(Result.Success(definition));
        var dialogSubmitterMock1 = Substitute.For<IDialogSubmitter>();
        dialogSubmitterMock1.SupportsDialog(dialog.DefinitionId, dialog.DefinitionVersion).Returns(true);
        using var provider = new ServiceCollection()
            .AddDialogFramework()
            .AddSingleton(dialogSubmitterMock1)
            .AddSingleton(_dialogSubmitterMock)
            .AddSingleton(_dialogRepositoryMock)
            .BuildServiceProvider(true);
        using var scope = provider.CreateScope();
        var sut = scope.ServiceProvider.GetRequiredService<IDialogService>();

        // Act
        var result = sut.Submit(dialog);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Should().BeSameAs(resultDialog);
    }

    [Fact]
    public void Submit_Returns_Result_Of_Validate_When_Unsuccessful()
    {
        // Arrange
        var dialog = TestDialogFactory.CreateEmpty();
        _dialogRepositoryMock.Get(dialog.DefinitionId, dialog.DefinitionVersion).Returns(Result.Error<DialogDefinition>("Kaboom"));

        // Act
        var result = CreateSut().Submit(dialog);

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Kaboom");
    }

    [Fact]
    public void Validate_Returns_Result_Of_Repository_When_Unsuccessful()
    {
        // Arrange
        var dialog = TestDialogFactory.CreateEmpty();
        _dialogRepositoryMock.Get(dialog.DefinitionId, dialog.DefinitionVersion).Returns(Result.Error<DialogDefinition>("Kaboom"));

        // Act
        var result = CreateSut().Validate(dialog);

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Kaboom");
    }

    [Fact]
    public void Validate_Returns_ValidationError_When_RequiredField_Is_Not_Filled()
    {
        // Arrange
        var dialog = TestDialogFactory.CreateDialogWithRequiredQuestion(answer: null);
        var definition = TestDialogDefinitionFactory.CreateDialogWithRequiredQuestion();
        _dialogRepositoryMock.Get(dialog.DefinitionId, dialog.DefinitionVersion).Returns(Result.Success(definition));

        // Act
        var result = CreateSut().Validate(dialog);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle();
        result.ValidationErrors.First().ErrorMessage.Should().Be("The Question field is required.");
    }

    [Fact]
    public void Validate_Returns_Success_When_All_Is_Good()
    {
        // Arrange
        var dialog = TestDialogFactory.CreateDialogWithRequiredQuestion(answer: "Some value");
        var definition = TestDialogDefinitionFactory.CreateDialogWithRequiredQuestion();
        _dialogRepositoryMock.Get(dialog.DefinitionId, dialog.DefinitionVersion).Returns(Result.Success(definition));

        // Act
        var result = CreateSut().Validate(dialog);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public void Validate_Returns_Result_From_Validation_When_Status_Is_Error()
    {
        // Arrange
        var dialog = TestDialogFactory.CreateDialogWithRequiredQuestion(answer: "Some value");
        var definition = TestDialogDefinitionFactory.CreateDialogWithCustomDialogPart(new MyMalfunctioningDialogPart("Question", null, "title"));
        _dialogRepositoryMock.Get(dialog.DefinitionId, dialog.DefinitionVersion).Returns(Result.Success(definition));

        // Act
        var result = CreateSut().Validate(dialog);

        // Assert
        result.Status.Should().Be(ResultStatus.Error);
        result.ErrorMessage.Should().Be("Kaboom");
    }

    private IDialogService CreateSut() => _scope.ServiceProvider.GetRequiredService<IDialogService>();

    public void Dispose()
    {
        _scope.Dispose();
        _provider.Dispose();
    }

    private sealed class MyMalfunctioningDialogPart : DialogPart, IValidatableDialogPart
    {
        public MyMalfunctioningDialogPart(string id, Evaluatable? condition, string title) : base(id, condition, title)
        {
        }

        public override DialogPartBuilder ToBuilder()
        {
            throw new NotImplementedException();
        }

        public Result Validate<T>(T value, Dialog dialog) => Result.Error("Kaboom");
    }
}
