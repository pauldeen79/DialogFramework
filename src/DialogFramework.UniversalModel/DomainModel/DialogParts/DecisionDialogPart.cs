using System;
using System.Linq;
using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;
using ExpressionFramework.Abstractions;

namespace DialogFramework.UniversalModel.DomainModel.DialogParts
{
    public partial record DecisionDialogPart
    {
        public string GetNextPartId(IDialogContext context, IDialog dialog, IConditionEvaluator evaluator)
        {
            var ctx = new Tuple<IDialogContext, IDialog>(context, dialog);
            return Decisions.FirstOrDefault(x => evaluator.Evaluate(ctx, x.Conditions))?.NextPartId
                ?? throw new NotSupportedException("There is no decision for this path");
        }
    }
}
