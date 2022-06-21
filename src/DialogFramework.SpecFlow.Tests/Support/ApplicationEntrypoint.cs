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
            .AddScoped<IDialogDefinitionRepository, TestDialogDefinitionRepository>()
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
    {
        _scope = Provider.CreateScope();
    }

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
    {
        get
        {
            if (_provider == null)
            {
                throw new InvalidOperationException("Something bad happened, application has not been initialized!");
            }
            return _provider;
        }
    }

    public static IDialogApplicationService Instance
        => Provider.GetRequiredService<IDialogApplicationService>();
}
