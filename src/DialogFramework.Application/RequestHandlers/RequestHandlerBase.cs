namespace DialogFramework.Application.RequestHandlers;

public abstract class RequestHandlerBase
{
    protected IDialogFactory DialogFactory { get; private set; }
    protected IDialogDefinitionProvider DialogDefinitionProvider { get; private set; }
    protected IConditionEvaluator ConditionEvaluator { get; private set; }
    protected ILogger Logger { get; private set; }

    protected RequestHandlerBase(IDialogFactory dialogFactory,
                                 IDialogDefinitionProvider dialogDefinitionProvider,
                                 IConditionEvaluator conditionEvaluator,
                                 ILogger logger)
    {
        DialogFactory = dialogFactory;
        DialogDefinitionProvider = dialogDefinitionProvider;
        ConditionEvaluator = conditionEvaluator;
        Logger = logger;
    }

    protected async Task<Result<IDialog>> PerformAction(IDialog dialog,
                                                        string operationName,
                                                        IRequestHandler<StartRequest, Result<IDialog>> startRequestHandler,
                                                        Func<IDialogDefinition, Result> action)
    {
        var dialogDefinitionResult = GetDialogDefinition(dialog.CurrentDialogId);
        if (!dialogDefinitionResult.IsSuccessful())
        {
            return Result<IDialog>.FromExistingResult(dialogDefinitionResult);
        }
        var definition = dialogDefinitionResult.Value!;
        return await PerformAction(dialog, operationName, startRequestHandler, action, definition);
    }

    protected async Task<Result<IDialog>> PerformAction(IDialog dialog,
                                                        string operationName,
                                                        IRequestHandler<StartRequest, Result<IDialog>> startRequestHandler,
                                                        Func<IDialogDefinition, Result> action,
                                                        IDialogDefinition? definition)
    {
        if (definition == null)
        {
            return await PerformAction(dialog, operationName, startRequestHandler, action);
        }

        try
        {
            var result = action.Invoke(definition);
            if (result.Status == ResultStatus.Redirect
                && result is Result<IDialogDefinitionIdentifier> dialogDefinitionIdentifierResult)
            {
                return await startRequestHandler.Handle(new StartRequest(dialogDefinitionIdentifierResult.Value ?? throw new InvalidOperationException("No dialog definition identifier found"), dialog.GetAllResults(definition), dialog.GetProperties()), CancellationToken.None);
            }

            if (!result.IsSuccessful())
            {
                return Result<IDialog>.FromExistingResult(result);
            }

            return Result<IDialog>.Success(dialog);
        }
        catch (Exception ex)
        {
            var msg = $"{operationName} failed";
            Logger.LogError(ex, msg);
            dialog.Error(definition!, ConditionEvaluator, new Error(msg));
            return Result<IDialog>.Success(dialog);
        }
    }

    protected Result<IDialogDefinition> GetDialogDefinition(IDialogDefinitionIdentifier dialogDefinitionIdentifier)
    {
        Result<IDialogDefinition>? definitionResult = null;
        try
        {
            definitionResult = DialogDefinitionProvider.GetDialogDefinition(dialogDefinitionIdentifier);
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(GetDialogDefinition)} failed";
            Logger.LogError(ex, msg);
            return Result<IDialogDefinition>.Error("Could not retrieve dialog definition");
        }

        if (definitionResult.Status == ResultStatus.NotFound)
        {
            var msg = $"Unknown dialog definition: Id [{dialogDefinitionIdentifier.Id}], Version [{dialogDefinitionIdentifier.Version}]";
            Logger.LogError(msg);
            return Result<IDialogDefinition>.NotFound(msg);
        }

        return definitionResult;
    }
}
