namespace DialogFramework.Domain.Expressions;

public class GetDialogPartResultIdsByPartExpressionBuilder : IExpressionBuilder
{
    public GetDialogPartResultIdsByPartExpressionBuilder() { }

    public GetDialogPartResultIdsByPartExpressionBuilder(GetDialogPartResultIdsByPartExpression getDialogPartResultIdsByPartExpression)
    {
        Function = getDialogPartResultIdsByPartExpression.Function?.ToBuilder();
        DialogPartId = new DialogPartIdentifierBuilder(getDialogPartResultIdsByPartExpression.DialogPartId);
    }

    public IExpressionFunctionBuilder? Function { get; set; }
    public DialogPartIdentifierBuilder DialogPartId { get; set; } = new();

    public IExpression Build()
        => new GetDialogPartResultIdsByPartExpression(Function?.Build(), DialogPartId.Build());

    public GetDialogPartResultIdsByPartExpressionBuilder WithDialogPartId(IDialogPartIdentifier id)
        => this.With(x => x.DialogPartId = new DialogPartIdentifierBuilder(id));

    public GetDialogPartResultIdsByPartExpressionBuilder WithDialogPartId(DialogPartIdentifierBuilder id)
        => this.With(x => x.DialogPartId = id);
}
