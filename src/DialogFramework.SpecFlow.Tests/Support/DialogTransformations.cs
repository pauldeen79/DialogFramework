namespace DialogFramework.SpecFlow.Tests.Support;

[Binding]
public static partial class DialogTransformations
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
        => table.CreateSet<TableDialogPartResultAnswer>().ToArray();
}
