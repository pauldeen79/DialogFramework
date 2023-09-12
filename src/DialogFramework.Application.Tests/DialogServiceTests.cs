namespace DialogFramework.Application.Tests;

public sealed class DialogServiceTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;
    private readonly Mock<IDialogSubmitter> _dialogSubmitterMock;
    private readonly Mock<IDialogRepository> _dialogRepositoryMock;

    public DialogServiceTests()
    {
        _dialogSubmitterMock = new();
        _dialogRepositoryMock = new();
        _provider = new ServiceCollection().AddDialogFramework(x =>
            x.AddScoped(_ => _dialogSubmitterMock.Object)
             .AddScoped(_ => _dialogRepositoryMock.Object)
            ).BuildServiceProvider(true);

        _scope = _provider.CreateScope();
    }

    [Fact]
    public void Submit_Returns_NotSupported_When_DialogDefinition_Has_No_Submitter()
    {
        // Arrange
        var dialog = TestDialogFactory.CreateEmpty();
        var definition = TestDialogDefinitionFactory.CreateEmpty();
        _dialogSubmitterMock.Setup(x => x.SupportsDialog(dialog.DefinitionId, dialog.DefinitionVersion)).Returns(false);
        _dialogRepositoryMock.Setup(x => x.Get(dialog.DefinitionId, dialog.DefinitionVersion)).Returns(Result<DialogDefinition>.Success(definition));

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
        _dialogSubmitterMock.Setup(x => x.SupportsDialog(dialog.DefinitionId, dialog.DefinitionVersion)).Returns(true);
        _dialogSubmitterMock.Setup(x => x.Submit(It.IsAny<Dialog>())).Returns(Result<Dialog>.Success(resultDialog));
        _dialogRepositoryMock.Setup(x => x.Get(dialog.DefinitionId, dialog.DefinitionVersion)).Returns(Result<DialogDefinition>.Success(definition));
        var dialogSubmitterMock1 = new Mock<IDialogSubmitter>();
        dialogSubmitterMock1.Setup(x => x.SupportsDialog(dialog.DefinitionId, dialog.DefinitionVersion)).Returns(true);
        using var provider = new ServiceCollection().AddDialogFramework(x =>
            x.AddSingleton(typeof(IDialogSubmitter), dialogSubmitterMock1.Object)
             .AddSingleton(typeof(IDialogSubmitter), _dialogSubmitterMock.Object)
             .AddSingleton(typeof(IDialogRepository), _dialogRepositoryMock.Object)
             ).BuildServiceProvider();
        var sut = provider.GetRequiredService<IDialogService>();

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
        _dialogRepositoryMock.Setup(x => x.Get(dialog.DefinitionId, dialog.DefinitionVersion)).Returns(Result<DialogDefinition>.Error("Kaboom"));

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
        _dialogRepositoryMock.Setup(x => x.Get(dialog.DefinitionId, dialog.DefinitionVersion)).Returns(Result<DialogDefinition>.Error("Kaboom"));

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
        _dialogRepositoryMock.Setup(x => x.Get(dialog.DefinitionId, dialog.DefinitionVersion)).Returns(Result<DialogDefinition>.Success(definition));

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
        _dialogRepositoryMock.Setup(x => x.Get(dialog.DefinitionId, dialog.DefinitionVersion)).Returns(Result<DialogDefinition>.Success(definition));

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
        _dialogRepositoryMock.Setup(x => x.Get(dialog.DefinitionId, dialog.DefinitionVersion)).Returns(Result<DialogDefinition>.Success(definition));

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

    private sealed record MyMalfunctioningDialogPart : DialogPart, IValidatableDialogPart
    {
        public MyMalfunctioningDialogPart(DialogPart original) : base(original)
        {
        }

        public MyMalfunctioningDialogPart(string id, Evaluatable? condition, string title) : base(id, condition, title)
        {
        }

        public Result Validate<T>(T value, Dialog dialog) => Result<T>.Error("Kaboom");
    }
}
