namespace DialogFramework.Domain.TestData;

public static class TestDialogDefinitionFactory
{
    public static DialogDefinition Create() => new DialogDefinitionBuilder()
        .WithId("MyDialogDefinition")
        .WithName("My dialog definition")
        .Build();
}
