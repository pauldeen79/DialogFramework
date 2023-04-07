namespace DialogFramework.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDialogFramework(this IServiceCollection instance, Action<IServiceCollection>? configureAction = null)
        => instance
        .AddSingleton<IDialogService, DialogService>()
        .Then(() => configureAction?.Invoke(instance));
}
