namespace DialogFramework.SpecFlow.Tests.StepDefinitions;

[Binding]
public static class DialogTransforms
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
    public static DialogPartResultAnswerBuilder[] RecipientTransform(Table table)
        => table.CreateSet
        (
            row => new DialogPartResultAnswerBuilder()
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(row["Result"]))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(row["Value"]))
        ).ToArray();
}
