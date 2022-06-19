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

    //[StepArgumentTransformation(@"Number:(.*)")]
    //public static object NumberTransform(string value)
    //    => Convert.ToDecimal(value, CultureInfo.InvariantCulture);

    //[StepArgumentTransformation(@"YesNo:(.*)")]
    //public static object YesNoTransform(string value)
    //    => Convert.ToBoolean(value, CultureInfo.InvariantCulture);

    //[StepArgumentTransformation(@"Date:(.*)")]
    //[StepArgumentTransformation(@"DateTime:(.*)")]
    //public static object DateTimeTransform(string value)
    //    => Convert.ToDateTime(value, CultureInfo.InvariantCulture);

    //[StepArgumentTransformation(@"Text:(.*)")]
    //public static object TextTransform(string value)
    //    => value;
}
