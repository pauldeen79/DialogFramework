namespace DialogFramework.Domain.Expressions;

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
