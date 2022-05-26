namespace DialogFramework.Core.Expressions;

public class GetDialogPartResultValuesByPartExpression : IExpression
{
    public GetDialogPartResultValuesByPartExpression(IExpressionFunction? function, string dialogPartId)
    {
        Function = function;
        DialogPartId = dialogPartId;
    }

    public IExpressionFunction? Function { get; }
    public string DialogPartId { get; }

    public IExpressionBuilder ToBuilder()
        => new GetDialogPartResultValuesByPartExpressionBuilder(this);
}
