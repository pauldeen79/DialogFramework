namespace DialogFramework.Domain.ExpressionEvaluatorProviders;

public class GetDialogPartResultValuesByPartExpressionEvaluatorProvider : IExpressionEvaluatorProvider
{
    public bool TryEvaluate(object? item, IExpression expression, IExpressionEvaluator evaluator, out object? result)
    {
        if (expression is GetDialogPartResultValuesByPartExpression partValuesByPart)
        {
            var dialog = item as IDialog;
            if (dialog == null)
            {
                result = null;
            }
            else
            {
                result = dialog.GetDialogPartResultsByPartIdentifier(partValuesByPart.DialogPartId)
                    .GetValueOrThrow()
                    .Where(x => x.Value.Value != null)
                    .Select(x => x.Value.Value);
            }
            return true;
        }

        result = default;
        return false;
    }
}
