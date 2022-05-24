using System.Collections.Generic;
using CrossCutting.Common;
using ExpressionFramework.Abstractions.DomainModel;

namespace DialogFramework.UniversalModel.DomainModel
{
    public record Decision
    {
        public Decision(IEnumerable<ICondition> conditions, string nextPartId)
        {
            Conditions = new ValueCollection<ICondition>(conditions);
            NextPartId = nextPartId;
        }

        public ValueCollection<ICondition> Conditions { get; }
        public string NextPartId { get; }
    }
}
