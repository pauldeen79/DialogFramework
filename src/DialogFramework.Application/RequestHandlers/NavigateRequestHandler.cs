namespace DialogFramework.Application.RequestHandlers;

public class NavigateRequestHandler : RequestHandlerBase
{
    private readonly StartRequestHandler _startRequestHandler;

    public NavigateRequestHandler(IDialogFactory dialogFactory,
                                  IDialogDefinitionProvider dialogDefinitionProvider,
                                  IConditionEvaluator conditionEvaluator,
                                  ILogger logger,
                                  StartRequestHandler startRequestHandler)
        : base(dialogFactory, dialogDefinitionProvider, conditionEvaluator, logger)
    {
        _startRequestHandler = startRequestHandler;
    }

    public Result<IDialog> Handle(NavigateRequest request)
        => PerformAction
        (
            request.Dialog,
            nameof(request.Dialog.Continue),
            _startRequestHandler,
            dialogDefinition => request.Dialog.NavigateTo(dialogDefinition, request.NavigateToPartId),
            GetDialogDefinition(request.DialogDefinitionIdentifier).Value
        );
}
