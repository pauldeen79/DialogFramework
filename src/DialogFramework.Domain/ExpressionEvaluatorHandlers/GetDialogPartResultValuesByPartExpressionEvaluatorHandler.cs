namespace DialogFramework.Domain.ExpressionEvaluatorHandlers;

public class GetDialogPartResultValuesByPartExpressionEvaluatorHandler : IExpressionEvaluatorHandler
{
    public Result<object?> Handle(object? context, IExpression expression, IExpressionEvaluator evaluator)
    {
        if (expression is GetDialogPartResultValuesByPartExpression partValuesByPart)
        {
            var dialog = context as IDialog;
            if (dialog == null)
            {
                return Result<object?>.Success(null);
            }
            else
            {
                return Result<object?>.Success(dialog.GetDialogPartResultsByPartIdentifier(partValuesByPart.DialogPartId)
                    .GetValueOrThrow()
                    .Where(x => x.Value.Value != null)
                    .Select(x => x.Value.Value));
            }
        }

        return Result<object?>.NotSupported();
    }
}
