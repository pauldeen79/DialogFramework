namespace DialogFramework.Domain.ExpressionEvaluatorHandlers;

public class GetDialogPartResultIdsByPartExpressionEvaluatorHandler : IExpressionEvaluatorHandler
{
    public Result<object?> Handle(object? context, IExpression expression, IExpressionEvaluator evaluator)
    {
        if (expression is GetDialogPartResultIdsByPartExpression partIdsByPart)
        {
            var dialog = context as IDialog;
            if (dialog == null)
            {
                return Result<object?>.Success(null);
            }
            else
            {
                return Result<object?>.Success(dialog.GetDialogPartResultsByPartIdentifier(partIdsByPart.DialogPartId)
                    .GetValueOrThrow()
                    .Select(x => x.ResultId.Value));
            }
        }

        return Result<object?>.NotSupported();
    }
}
