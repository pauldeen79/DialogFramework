namespace DialogFramework.Application.RequestHandlers;

public class NavigateRequestHandler : RequestHandlerBase, IRequestHandler<NavigateRequest, Result<IDialog>>
{
    private readonly IRequestHandler<StartRequest, Result<IDialog>> _startRequestHandler;

    public NavigateRequestHandler(IDialogFactory dialogFactory,
                                  IDialogDefinitionProvider dialogDefinitionProvider,
                                  IConditionEvaluator conditionEvaluator,
                                  ILogger logger,
                                  IRequestHandler<StartRequest, Result<IDialog>> startRequestHandler)
        : base(dialogFactory, dialogDefinitionProvider, conditionEvaluator, logger)
    {
        _startRequestHandler = startRequestHandler;
    }

    public Task<Result<IDialog>> Handle(NavigateRequest request, CancellationToken cancellationToken)
        => PerformAction
        (
            request.Dialog,
            nameof(request.Dialog.NavigateTo),
            _startRequestHandler,
            dialogDefinition => request.Dialog.NavigateTo(dialogDefinition, request.NavigateToPartId, ConditionEvaluator),
            GetDialogDefinition(request.DialogDefinitionIdentifier).Value
        );
}
