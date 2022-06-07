namespace DialogFramework.Application;

public class DialogFactory : IDialogFactory
{
    public bool CanCreate(IDialogDefinition dialogDefinition) => dialogDefinition is DialogDefinition;

    public IDialog Create(IDialogDefinition dialogDefinition)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(dialogDefinition.Metadata))
            .WithCurrentPartId(new DialogPartIdentifierBuilder().WithValue("Empty"))
            .WithCurrentState(DialogState.Initial)
            .Build();
}
