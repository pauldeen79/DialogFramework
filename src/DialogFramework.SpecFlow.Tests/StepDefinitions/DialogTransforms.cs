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
}
