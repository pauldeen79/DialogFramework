namespace DialogFramework.Domain.Expressions;

public class GetDialogPartResultIdsByPartExpression : IExpression
{
    public GetDialogPartResultIdsByPartExpression(IExpressionFunction? function, IDialogPartIdentifier dialogPartId)
    {
        Function = function;
        DialogPartId = dialogPartId;
    }

    public IExpressionFunction? Function { get; }
    public IDialogPartIdentifier DialogPartId { get; }

    public IExpressionBuilder ToBuilder()
        => new GetDialogPartResultIdsByPartExpressionBuilder(this);
}
