using System.Collections.Generic;
using System.Linq;
using ExpressionFramework.Abstractions.DomainModel.Builders;

namespace DialogFramework.UniversalModel.DomainModel
{
    public class DecisionBuilder
    {
        public string NextPartId { get; set; } = string.Empty;
        public List<IConditionBuilder> Conditions { get; set; } = new List<IConditionBuilder>();

        public Decision Build() => new Decision(Conditions.Select(x => x.Build()), NextPartId);

        public DecisionBuilder AddConditions(params IConditionBuilder[] conditions)
        {
            Conditions.AddRange(conditions);
            return this;
        }

        public DecisionBuilder AddConditions(IEnumerable<IConditionBuilder> conditions)
        {
            Conditions.AddRange(conditions);
            return this;
        }

        public DecisionBuilder WithNextPartId(string id)
        {
            NextPartId = id;
            return this;
        }
    }
}
