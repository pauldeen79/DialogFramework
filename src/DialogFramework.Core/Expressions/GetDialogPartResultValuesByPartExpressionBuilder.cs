﻿namespace DialogFramework.Core.Expressions;

public class GetDialogPartResultValuesByPartExpressionBuilder : IExpressionBuilder
{
    public GetDialogPartResultValuesByPartExpressionBuilder()
    {
        DialogPartId = string.Empty;
    }

    public GetDialogPartResultValuesByPartExpressionBuilder(GetDialogPartResultValuesByPartExpression getDialogPartResultValuesByPartExpression)
    {
        Function = getDialogPartResultValuesByPartExpression.Function?.ToBuilder();
        DialogPartId = getDialogPartResultValuesByPartExpression.DialogPartId;
    }

    public IExpressionFunctionBuilder? Function { get; set; }
    public string DialogPartId { get; set; }

    public IExpression Build()
        => new GetDialogPartResultValuesByPartExpression(Function?.Build(), DialogPartId);

    public GetDialogPartResultValuesByPartExpressionBuilder WithDialogPartId(string id)
    {
        DialogPartId = id;
        return this;
    }
}