namespace DialogFramework.Application.RequestHandlers;

public class AbortRequestHandler : RequestHandlerBase
{
    private readonly StartRequestHandler _startRequestHandler;

    public AbortRequestHandler(IDialogFactory dialogFactory,
                               IDialogDefinitionProvider dialogDefinitionProvider,
                               IConditionEvaluator conditionEvaluator,
                               ILogger logger,
                               StartRequestHandler startRequestHandler)
        : base(dialogFactory, dialogDefinitionProvider, conditionEvaluator, logger)
    {
        _startRequestHandler = startRequestHandler;
    }

    public Result<IDialog> Handle(AbortRequest request)
        => PerformAction
        (
            request.Dialog,
            nameof(request.Dialog.Continue),
            _startRequestHandler,
            dialogDefinition => request.Dialog.Abort(dialogDefinition)
        );
}
