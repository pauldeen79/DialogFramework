namespace DialogFramework.Domain.ExpressionEvaluatorProviders;

public class GetDialogPartResultValuesByPartExpressionEvaluatorProvider : IExpressionEvaluatorProvider
{
    public bool TryEvaluate(object? item, IExpression expression, IExpressionEvaluator evaluator, out object? result)
    {
        if (expression is GetDialogPartResultValuesByPartExpression partValuesByPart)
        {
            var context = item as IDialog;
            if (context == null)
            {
                result = null;
            }
            else
            {
                result = context.GetDialogPartResultsByPartIdentifier(partValuesByPart.DialogPartId)
                    .Where(x => x.Value.Value != null)
                    .Select(x => x.Value.Value);
            }
            return true;
        }

        result = default;
        return false;
    }
}
