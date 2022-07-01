namespace DialogFramework.SpecFlow.Tests.Support;

[Binding]
public sealed class ApplicationEntrypoint
{
    private static Mock<ILogger>? _loggerMock;
    private static ServiceProvider? _provider;
    private IServiceScope? _scope;

    [BeforeTestRun]
    public static void SetupApplication()
    {
        _loggerMock = new Mock<ILogger>();
        _provider = new ServiceCollection()
            .AddDialogFramework()
            .AddScoped<IDialogDefinitionProvider, TestDialogDefinitionProvider>()
            .AddScoped(_ => _loggerMock.Object)
            .BuildServiceProvider();
    }

    [AfterTestRun]
    public static void CleanUpApplication()
    {
        if (_provider != null)
        {
            _loggerMock = null;
            _provider.Dispose();
            _provider = null;
        }
    }

    [BeforeScenario]
    public void SetupScenario()
        => _scope = Provider.CreateScope();

    [AfterScenario]
    public void CleanUpScenario()
    {
        if (_scope != null)
        {
            _scope.Dispose();
            _scope = null;
        }
    }

    private static IServiceProvider Provider
        => _provider == null
            ? throw new InvalidOperationException("Something bad happened, application has not been initialized!")
            : (IServiceProvider)_provider;

    public static IRequestHandler<StartRequest, Result<IDialog>> StartHandler
        => Provider.GetRequiredService<IRequestHandler<StartRequest, Result<IDialog>>>();

    public static IRequestHandler<ContinueRequest, Result<IDialog>> ContinueHandler
        => Provider.GetRequiredService<IRequestHandler<ContinueRequest, Result<IDialog>>>();

    public static IRequestHandler<AbortRequest, Result<IDialog>> AbortHandler
        => Provider.GetRequiredService<IRequestHandler<AbortRequest, Result<IDialog>>>();

    public static IRequestHandler<NavigateRequest, Result<IDialog>> NavigateHandler
        => Provider.GetRequiredService<IRequestHandler<NavigateRequest, Result<IDialog>>>();

    public static IRequestHandler<ResetStateRequest, Result<IDialog>> ResetStateHandler
        => Provider.GetRequiredService<IRequestHandler<ResetStateRequest, Result<IDialog>>>();
}
