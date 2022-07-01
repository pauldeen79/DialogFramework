namespace DialogFramework.Application.RequestHandlers;

public class AbortRequestHandler : RequestHandlerBase, IRequestHandler<AbortRequest, Result<IDialog>>
{
    private readonly IRequestHandler<StartRequest, Result<IDialog>> _startRequestHandler;

    public AbortRequestHandler(IDialogFactory dialogFactory,
                               IDialogDefinitionProvider dialogDefinitionProvider,
                               IConditionEvaluator conditionEvaluator,
                               ILogger logger,
                               IRequestHandler<StartRequest, Result<IDialog>> startRequestHandler)
        : base(dialogFactory, dialogDefinitionProvider, conditionEvaluator, logger)
    {
        _startRequestHandler = startRequestHandler;
    }

    public Task<Result<IDialog>> Handle(AbortRequest request, CancellationToken cancellationToken)
        => PerformAction
        (
            request.Dialog,
            nameof(request.Dialog.Abort),
            _startRequestHandler,
            dialogDefinition => request.Dialog.Abort(dialogDefinition)
        );
}
