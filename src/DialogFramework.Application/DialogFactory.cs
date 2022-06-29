namespace DialogFramework.Application;

public class DialogFactory : IDialogFactory
{
    public bool CanCreate(IDialogDefinition dialogDefinition,
                          IEnumerable<IDialogPartResult> dialogPartResults)
        => dialogDefinition is DialogDefinition;

    public IDialog Create(IDialogDefinition dialogDefinition,
                          IEnumerable<IDialogPartResult> dialogPartResults)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(dialogDefinition.Metadata))
            .WithCurrentPartId(new DialogPartIdentifierBuilder().WithValue("Empty"))
            .WithCurrentState(DialogState.Initial)
            .AddResults(dialogPartResults.Select(x => new DialogPartResultBuilder(x)))
            .Build();
}
