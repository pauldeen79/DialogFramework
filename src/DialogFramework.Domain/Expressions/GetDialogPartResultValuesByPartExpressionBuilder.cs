namespace DialogFramework.Domain.Expressions;

public class GetDialogPartResultValuesByPartExpressionBuilder : IExpressionBuilder
{
    public GetDialogPartResultValuesByPartExpressionBuilder() { }
    
    public GetDialogPartResultValuesByPartExpressionBuilder(GetDialogPartResultValuesByPartExpression getDialogPartResultValuesByPartExpression)
    {
        Function = getDialogPartResultValuesByPartExpression.Function?.ToBuilder();
        DialogPartId = new DialogPartIdentifierBuilder(getDialogPartResultValuesByPartExpression.DialogPartId);
    }

    public IExpressionFunctionBuilder? Function { get; set; }
    public DialogPartIdentifierBuilder DialogPartId { get; set; } = new();

    public IExpression Build()
        => new GetDialogPartResultValuesByPartExpression(Function?.Build(), DialogPartId.Build());

    public GetDialogPartResultValuesByPartExpressionBuilder WithDialogPartId(IDialogPartIdentifier id)
        => this.With(x => x.DialogPartId = new DialogPartIdentifierBuilder(id));

    public GetDialogPartResultValuesByPartExpressionBuilder WithDialogPartId(DialogPartIdentifierBuilder id)
        => this.With(x => x.DialogPartId = id);
}
