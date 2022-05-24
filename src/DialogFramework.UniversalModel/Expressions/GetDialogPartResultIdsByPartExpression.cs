using ExpressionFramework.Abstractions.DomainModel;
using ExpressionFramework.Abstractions.DomainModel.Builders;

namespace DialogFramework.UniversalModel.Expressions
{
    public class GetDialogPartResultIdsByPartExpression : IExpression
    {
        public GetDialogPartResultIdsByPartExpression(IExpressionFunction? function, string dialogPartId)
        {
            Function = function;
            DialogPartId = dialogPartId;
        }

        public IExpressionFunction? Function { get; }
        public string DialogPartId { get; }

        public IExpressionBuilder ToBuilder()
            => new GetDialogPartResultIdsByPartExpressionBuilder(this);
    }
}
