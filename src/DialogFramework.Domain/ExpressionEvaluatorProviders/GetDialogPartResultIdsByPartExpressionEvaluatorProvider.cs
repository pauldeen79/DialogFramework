namespace DialogFramework.Domain.ExpressionEvaluatorProviders;

public class GetDialogPartResultIdsByPartExpressionEvaluatorProvider : IExpressionEvaluatorProvider
{
    public bool TryEvaluate(object? item, IExpression expression, IExpressionEvaluator evaluator, out object? result)
    {
        if (expression is GetDialogPartResultIdsByPartExpression partIdsByPart)
        {
            var context = item as IDialog;
            if (context == null)
            {
                result = null;
            }
            else
            {
                result = context.GetDialogPartResultsByPartIdentifier(partIdsByPart.DialogPartId).Select(x => x.ResultId.Value);
            }
            return true;
        }

        result = default;
        return false;
    }
}
