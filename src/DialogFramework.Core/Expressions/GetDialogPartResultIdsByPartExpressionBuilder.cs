namespace DialogFramework.Core.Expressions;

public class GetDialogPartResultIdsByPartExpressionBuilder : IExpressionBuilder
{
    public GetDialogPartResultIdsByPartExpressionBuilder()
    {
        DialogPartId = string.Empty;
    }

    public GetDialogPartResultIdsByPartExpressionBuilder(GetDialogPartResultIdsByPartExpression getDialogPartResultIdsByPartExpression)
    {
        Function = getDialogPartResultIdsByPartExpression.Function?.ToBuilder();
        DialogPartId = getDialogPartResultIdsByPartExpression.DialogPartId;
    }

    public IExpressionFunctionBuilder? Function { get; set; }
    public string DialogPartId { get; set; }

    public IExpression Build()
        => new GetDialogPartResultIdsByPartExpression(Function?.Build(), DialogPartId);

    public GetDialogPartResultIdsByPartExpressionBuilder WithDialogPartId(string id)
    {
        DialogPartId = id;
        return this;
    }
}
