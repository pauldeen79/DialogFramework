namespace DialogFramework.Core.ExpressionEvaluatorProviders;

public class GetDialogPartResultValuesByPartExpressionEvaluatorProvider : IExpressionEvaluatorProvider
{
    public bool TryEvaluate(object? item, IExpression expression, IExpressionEvaluator evaluator, out object? result)
    {
        if (expression is GetDialogPartResultValuesByPartExpression partValuesByPart)
        {
            var tuple = item as Tuple<IDialogContext, IDialog>;
            if (tuple == null)
            {
                result = null;
            }
            else
            {
                var context = tuple.Item1;
                var dialog = tuple.Item2;
                result = context.GetDialogPartResultsByPart(dialog.Parts.Single(x => x.Id == partValuesByPart.DialogPartId))
                    .Where(x => x.Value.Value != null)
                    .Select(x => x.Value.Value);
            }
            return true;
        }

        result = default;
        return false;
    }
}
