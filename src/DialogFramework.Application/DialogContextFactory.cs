namespace DialogFramework.Application;

public class DialogContextFactory : IDialogFactory
{
    public bool CanCreate(IDialogDefinition dialog) => dialog is DialogDefinition;

    public IDialog Create(IDialogDefinition dialog)
        => new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(dialog.Metadata))
            .WithCurrentPartId(new DialogPartIdentifierBuilder().WithValue("Empty"))
            .WithCurrentState(DialogState.Initial)
            .Build();
}
