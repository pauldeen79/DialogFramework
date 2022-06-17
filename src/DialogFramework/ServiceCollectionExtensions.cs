namespace DialogFramework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDialogFramework(this IServiceCollection services)
        => services.AddExpressionFramework
        (
            x => x.AddSingleton<IExpressionEvaluatorProvider, GetDialogPartResultIdsByPartExpressionEvaluatorProvider>()
                  .AddSingleton<IExpressionEvaluatorProvider, GetDialogPartResultValuesByPartExpressionEvaluatorProvider>()
        )
        .AddScoped<IDialogFactory, DialogFactory>()
        .AddScoped<IDialogApplicationService, DialogApplicationService>();
}
