namespace DialogFramework.Application.Tests;

public sealed class DialogServiceTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly Mock<IDialogSubmitter> _dialogSubmitterMock;
    private readonly Mock<IDialogRepository> _dialogRepositoryMock;

    public DialogServiceTests()
    {
        _dialogSubmitterMock = new();
        _dialogRepositoryMock = new();
        _provider = new ServiceCollection().AddDialogFramework(x =>
            x.AddSingleton(typeof(IDialogSubmitter), _dialogSubmitterMock.Object)
             .AddSingleton(typeof(IDialogRepository), _dialogRepositoryMock.Object)
            ).BuildServiceProvider();
    }

    [Fact]
    public void Submit_Returns_NotSupported_When_DialogDefinition_Has_No_Submitter()
    {
        // Arrange
        var dialog = TestDialogFactory.Create();
        var definition = TestDialogDefinitionFactory.Create();
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
        var dialog = TestDialogFactory.Create();
        var definition = TestDialogDefinitionFactory.Create();
        var resultDialog = TestDialogFactory.Create();
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

    private IDialogService CreateSut() => _provider.GetRequiredService<IDialogService>();

    public void Dispose() => _provider.Dispose();
}
