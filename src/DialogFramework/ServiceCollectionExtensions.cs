namespace DialogFramework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDialogFramework(this IServiceCollection services)
        => services.AddExpressionFramework
        (
            x => x.AddSingleton<IExpressionEvaluatorHandler, GetDialogPartResultIdsByPartExpressionEvaluatorHandler>()
                  .AddSingleton<IExpressionEvaluatorHandler, GetDialogPartResultValuesByPartExpressionEvaluatorHandler>()
        )
        .AddScoped<IDialogFactory, DialogFactory>()
        .AddMediatR(typeof(StartRequestHandler).Assembly);
}
