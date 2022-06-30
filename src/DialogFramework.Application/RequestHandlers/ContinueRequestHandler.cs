namespace DialogFramework.Application.RequestHandlers;

public class ContinueRequestHandler : RequestHandlerBase
{
    private readonly StartRequestHandler _startRequestHandler;

    public ContinueRequestHandler(IDialogFactory dialogFactory,
                                  IDialogDefinitionProvider dialogDefinitionProvider,
                                  IConditionEvaluator conditionEvaluator,
                                  ILogger logger,
                                  StartRequestHandler startRequestHandler)
        : base(dialogFactory, dialogDefinitionProvider, conditionEvaluator, logger)
    {
        _startRequestHandler = startRequestHandler;
    }

    public Result<IDialog> Handle(ContinueRequest request)
        => PerformAction
        (
            request.Dialog,
            nameof(request.Dialog.Continue),
            _startRequestHandler,
            dialogDefinition => request.Dialog.Continue(dialogDefinition, request.DialogPartResults, ConditionEvaluator)
        );
}
