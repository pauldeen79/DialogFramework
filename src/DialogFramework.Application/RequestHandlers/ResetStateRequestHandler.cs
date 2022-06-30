namespace DialogFramework.Application.RequestHandlers;

public class ResetStateRequestHandler : RequestHandlerBase
{
    private readonly StartRequestHandler _startRequestHandler;

    public ResetStateRequestHandler(IDialogFactory dialogFactory,
                                    IDialogDefinitionProvider dialogDefinitionProvider,
                                    IConditionEvaluator conditionEvaluator,
                                    ILogger logger,
                                    StartRequestHandler startRequestHandler)
        : base(dialogFactory, dialogDefinitionProvider, conditionEvaluator, logger)
    {
        _startRequestHandler = startRequestHandler;
    }

    public Result<IDialog> Handle(ResetStateRequest request)
        => PerformAction
        (
            request.Dialog,
            nameof(request.Dialog.Continue),
            _startRequestHandler,
            dialogDefinition => request.Dialog.ResetCurrentState(dialogDefinition)
        );
}
