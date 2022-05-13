namespace DialogFramework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDialogFramework(this IServiceCollection services)
        => services.AddTransient<IDialogContextFactory, DialogContextFactory>()
                   .AddTransient<IDialogService, DialogService>();
}
