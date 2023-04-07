namespace DialogFramework.Domain.TestData;

public static class TestDialogFactory
{
    public static Dialog Create() => new DialogBuilder().WithDefinitionId("MyDialogDefinition").WithDefinitionVersion("1.0.0").WithId("Correct").Build();
}
