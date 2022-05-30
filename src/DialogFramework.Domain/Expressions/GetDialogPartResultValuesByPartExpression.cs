namespace DialogFramework.Domain.Expressions;

public class GetDialogPartResultValuesByPartExpression : IExpression
{
    public GetDialogPartResultValuesByPartExpression(IExpressionFunction? function, IDialogPartIdentifier dialogPartId)
    {
        Function = function;
        DialogPartId = dialogPartId;
    }

    public IExpressionFunction? Function { get; }
    public IDialogPartIdentifier DialogPartId { get; }

    public IExpressionBuilder ToBuilder()
        => new GetDialogPartResultValuesByPartExpressionBuilder(this);
}
