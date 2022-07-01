namespace DialogFramework.Tests;

public abstract class TestBase : IDisposable
{
    private bool disposedValue;
    private readonly ServiceProvider _provider;
    protected Mock<ILogger> LoggerMock { get; }
    protected IDialogDefinition SimpleFormFlowDialogDefinition { get; }
    protected IDialogDefinition TestFlowDialogDefinition { get; }
    protected IRequestHandler<StartRequest, Result<IDialog>> StartHandler { get; }
    protected IRequestHandler<ContinueRequest, Result<IDialog>> ContinueHandler { get; }
    protected IRequestHandler<NavigateRequest, Result<IDialog>> NavigateHandler { get; }

    protected TestBase()
    {
        LoggerMock = new Mock<ILogger>();
        _provider = new ServiceCollection()
            .AddDialogFramework()
            .AddSingleton<IDialogDefinitionProvider, TestDialogDefinitionProvider>()
            .AddSingleton(LoggerMock.Object)
            .BuildServiceProvider();
        SimpleFormFlowDialogDefinition = _provider.GetRequiredService<IDialogDefinitionProvider>().GetDialogDefinition(new DialogDefinitionIdentifier(nameof(SimpleFormFlowDialog), "1.0.0")).GetValueOrThrow();
        TestFlowDialogDefinition = _provider.GetRequiredService<IDialogDefinitionProvider>().GetDialogDefinition(new DialogDefinitionIdentifier(nameof(TestFlowDialog), "1.0.0")).GetValueOrThrow();
        StartHandler = _provider.GetRequiredService<IRequestHandler<StartRequest, Result<IDialog>>>();
        ContinueHandler = _provider.GetRequiredService<IRequestHandler<ContinueRequest, Result<IDialog>>>();
        NavigateHandler = _provider.GetRequiredService<IRequestHandler<NavigateRequest, Result<IDialog>>>();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _provider.Dispose();
            }

            disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
