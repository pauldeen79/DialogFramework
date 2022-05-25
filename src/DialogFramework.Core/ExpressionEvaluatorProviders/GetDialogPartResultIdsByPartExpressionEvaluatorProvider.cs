﻿namespace DialogFramework.Core.ExpressionEvaluatorProviders;

public class GetDialogPartResultIdsByPartExpressionEvaluatorProvider : IExpressionEvaluatorProvider
{
    public bool TryEvaluate(object? item, IExpression expression, IExpressionEvaluator evaluator, out object? result)
    {
        if (expression is GetDialogPartResultIdsByPartExpression partIdsByPart)
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
                result = context.GetDialogPartResultsByPart(dialog.Parts.Single(x => x.Id == partIdsByPart.DialogPartId)).Select(x => x.ResultId);
            }
            return true;
        }

        result = default;
        return false;
    }
}