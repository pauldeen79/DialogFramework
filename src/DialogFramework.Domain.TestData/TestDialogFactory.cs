namespace DialogFramework.Domain.TestData;

public static class TestDialogFactory
{
    public static Dialog CreateEmpty(string id = "Correct")
        => new DialogBuilder()
            .WithDefinitionId("MyDialogDefinition")
            .WithId(id)
            .Build();

    public static Dialog CreateDialogWithRequiredQuestion(string? answer)
        => new DialogBuilder()
            .WithDefinitionId("MyDialogWithRequiredQuestion")
            .WithId(Guid.NewGuid().ToString())
            .AddResults(answer is null
                ? Array.Empty<DialogPartResultBuilder>() 
                : new[] { new SingleQuestionDialogPartResultBuilder<string>().WithPartId("Question").WithValue(answer) }
            )
            .Build();
}
