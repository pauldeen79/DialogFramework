namespace DialogFramework.Domain.TestData;

public static class TestDialogFactory
{
    public static Dialog Create(string id = "Correct")
        => new DialogBuilder()
            .WithDefinitionId("MyDialogDefinition")
            .WithDefinitionVersion(new Version(1, 0, 0))
            .WithId(id)
            .Build();
}
