﻿namespace DialogFramework.SpecFlow.Tests.Support;

[Binding]
public static class DialogTransformations
{
    [StepArgumentTransformation]
    public static IDialogDefinitionIdentifier DialogDefinitionIdentifierTransform(string value)
        => new DialogDefinitionIdentifierBuilder().WithId(value).WithVersion("1.0.0").Build();

    [StepArgumentTransformation]
    public static IDialogPartIdentifier DialogPartIdentifierTransform(string value)
        => new DialogPartIdentifierBuilder().WithValue(value).Build();

    [StepArgumentTransformation]
    public static IDialogPartResultIdentifier DialogPartResultIdentifierTransform(string value)
        => new DialogPartResultIdentifierBuilder().WithValue(value).Build();

    [StepArgumentTransformation]
    public static IDialogPartResultAnswer[] DialogPartResultAnswerTransform(Table table)
        => table.CreateSet
        (
            row => new DialogPartResultAnswerBuilder()
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(row["Result"]))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(row["Value"]))
                .Chain(x => x.Value.Value = ValueExpression.Evaluate(x.Value.Value))
        ).Select(x => x.Build()).ToArray();
}
