namespace DialogFramework.SpecFlow.Tests.Support;

[Binding]
public class DialogPartResultIdentifierValueRetriever : IValueRetriever
{
    [BeforeTestRun]
    public static void RegisterValueRetrievers()
        => Service.Instance.ValueRetrievers.Register<DialogPartResultIdentifierValueRetriever>();

    public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        => propertyType == typeof(IDialogPartResultIdentifier);

    public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        => new DialogPartResultIdentifierBuilder().WithValue(keyValuePair.Value).Build();
}
