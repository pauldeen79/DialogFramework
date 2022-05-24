using System;
using System.Linq;
using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;
using ExpressionFramework.Abstractions;
using ExpressionFramework.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DialogFramework.UniversalModel.DomainModel.DialogParts
{
    public partial record DecisionDialogPart
    {
        private static IConditionEvaluator? _evaluator;
        private static IConditionEvaluator Evaluator
        {
            get
            {
                if (_evaluator == null)
                {
                    var serviceCollection = new ServiceCollection();
                    serviceCollection.AddExpressionFramework();
                    var provider = serviceCollection.BuildServiceProvider();
                    _evaluator = provider.GetRequiredService<IConditionEvaluator>();
                }
                return _evaluator;
            }
        }

        public string GetNextPartId(IDialogContext context, IDialog dialog)
        {
            var ctx = new Tuple<IDialogContext, IDialog>(context, dialog);
            return Decisions.FirstOrDefault(x => Evaluator.Evaluate(ctx, x.Conditions))?.NextPartId
                ?? throw new NotSupportedException("There is no decision for this path");
        }
    }
}
