namespace DialogFramework.Application.RequestHandlers;

public class ContinueRequestHandler : RequestHandlerBase, IRequestHandler<ContinueRequest, Result<IDialog>>
{
    private readonly IRequestHandler<StartRequest, Result<IDialog>> _startRequestHandler;

    public ContinueRequestHandler(IDialogFactory dialogFactory,
                                  IDialogDefinitionProvider dialogDefinitionProvider,
                                  IConditionEvaluator conditionEvaluator,
                                  ILogger logger,
                                  IRequestHandler<StartRequest, Result<IDialog>> startRequestHandler)
        : base(dialogFactory, dialogDefinitionProvider, conditionEvaluator, logger)
    {
        _startRequestHandler = startRequestHandler;
    }

    public Task<Result<IDialog>> Handle(ContinueRequest request, CancellationToken cancellationToken)
        => PerformAction
        (
            request.Dialog,
            nameof(request.Dialog.Continue),
            _startRequestHandler,
            dialogDefinition => request.Dialog.Continue(dialogDefinition, request.DialogPartResults, ConditionEvaluator)
        );
}
