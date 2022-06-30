namespace DialogFramework.SpecFlow.Tests.Support;

[Binding]
public class DialogPartResultValueAnswerValueRetriever : IValueRetriever
{
    [BeforeTestRun]
    public static void RegisterValueRetrievers()
        => Service.Instance.ValueRetrievers.Register<DialogPartResultValueAnswerValueRetriever>();

    public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        => propertyType == typeof(IDialogPartResultValueAnswer);

    public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        => new DialogPartResultValueAnswerBuilder().WithValue(ValueExpression.Evaluate(keyValuePair.Value)).Build();
}
