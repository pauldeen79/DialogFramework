namespace DialogFramework.Application.RequestHandlers;

public class StartRequestHandler : RequestHandlerBase
{
    public StartRequestHandler(IDialogFactory dialogFactory,
                               IDialogDefinitionProvider dialogDefinitionProvider,
                               IConditionEvaluator conditionEvaluator,
                               ILogger logger)
        : base(dialogFactory, dialogDefinitionProvider, conditionEvaluator, logger)
    {
    }

    public Result<IDialog> Handle(StartRequest request)
    {
        var dialogResult = CreateDialogAndDefinition(request);
        if (!dialogResult.IsSuccessful())
        {
            return Result<IDialog>.FromExistingResult(dialogResult);
        }

        var dialog = dialogResult.Value!.Dialog;
        var definition = dialogResult.Value!.Definition;

        return PerformAction
        (
            dialog,
            nameof(dialog.Start),
            this,
            _ => dialog.Start(definition, ConditionEvaluator),
            definition
        );
    }

    private Result<(IDialog Dialog, IDialogDefinition Definition)> CreateDialogAndDefinition(StartRequest request)
    {
        var dialogDefinitionResult = GetDialogDefinition(request.DialogDefinitionIdentifier);
        if (!dialogDefinitionResult.IsSuccessful())
        {
            return Result<(IDialog Dialog, IDialogDefinition Definition)>.FromExistingResult(dialogDefinitionResult);
        }

        var dialogDefinition = dialogDefinitionResult.Value!;

        try
        {
            var createResult = DialogFactory.Create(dialogDefinition, request.DialogPartResults);
            return Result<(IDialog Dialog, IDialogDefinition Definition)>.Success((createResult.GetValueOrThrow(), dialogDefinition));
        }
        catch (Exception ex)
        {
            var msg = $"{nameof(CreateDialogAndDefinition)} failed";
            Logger.LogError(ex, msg);
            return Result<(IDialog Dialog, IDialogDefinition Definition)>.Error("Dialog creation failed");
        }
    }
}
