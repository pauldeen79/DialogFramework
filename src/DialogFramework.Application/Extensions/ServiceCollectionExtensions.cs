namespace DialogFramework.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDialogFramework(this IServiceCollection instance)
        => instance.AddScoped<IDialogService, DialogService>();
}
