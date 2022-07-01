namespace DialogFramework.Application.RequestHandlers;

public class ResetStateRequestHandler : RequestHandlerBase, IRequestHandler<ResetStateRequest, Result<IDialog>>
{
    private readonly IRequestHandler<StartRequest, Result<IDialog>> _startRequestHandler;

    public ResetStateRequestHandler(IDialogFactory dialogFactory,
                                    IDialogDefinitionProvider dialogDefinitionProvider,
                                    IConditionEvaluator conditionEvaluator,
                                    ILogger logger,
                                    IRequestHandler<StartRequest, Result<IDialog>> startRequestHandler)
        : base(dialogFactory, dialogDefinitionProvider, conditionEvaluator, logger)
    {
        _startRequestHandler = startRequestHandler;
    }

    public Task<Result<IDialog>> Handle(ResetStateRequest request, CancellationToken cancellationToken)
        => PerformAction
        (
            request.Dialog,
            nameof(request.Dialog.Continue),
            _startRequestHandler,
            dialogDefinition => request.Dialog.ResetCurrentState(dialogDefinition, request.DialogPartIdentifier),
            GetDialogDefinition(request.DialogDefinitionIdentifier).Value
        );
}
