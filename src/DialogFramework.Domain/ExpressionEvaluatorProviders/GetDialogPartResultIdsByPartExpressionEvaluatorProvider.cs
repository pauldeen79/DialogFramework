namespace DialogFramework.Domain.ExpressionEvaluatorProviders;

public class GetDialogPartResultIdsByPartExpressionEvaluatorProvider : IExpressionEvaluatorProvider
{
    public bool TryEvaluate(object? item, IExpression expression, IExpressionEvaluator evaluator, out object? result)
    {
        if (expression is GetDialogPartResultIdsByPartExpression partIdsByPart)
        {
            var dialog = item as IDialog;
            if (dialog == null)
            {
                result = null;
            }
            else
            {
                result = dialog.GetDialogPartResultsByPartIdentifier(partIdsByPart.DialogPartId)
                    .GetValueOrThrow()
                    .Select(x => x.ResultId.Value);
            }
            return true;
        }

        result = default;
        return false;
    }
}
