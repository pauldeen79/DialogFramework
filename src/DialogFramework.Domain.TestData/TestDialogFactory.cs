namespace DialogFramework.Domain.TestData;

public static class TestDialogFactory
{
    public static Dialog Create(string id = "Correct")
        => new DialogBuilder()
            .WithDefinitionId("MyDialogDefinition")
            .WithId(id)
            .Build();
}
