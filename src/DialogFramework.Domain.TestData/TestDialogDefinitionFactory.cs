namespace DialogFramework.Domain.TestData;

public static class TestDialogDefinitionFactory
{
    public static DialogDefinition Create() => new DialogDefinitionBuilder().WithId("MyDialogDefinition").WithVersion("1.0.0").Build();
}
