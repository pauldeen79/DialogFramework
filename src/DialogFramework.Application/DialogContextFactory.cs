namespace DialogFramework.Application;

public class DialogContextFactory : IDialogContextFactory
{
    public bool CanCreate(IDialogDefinition dialog) => dialog is DialogDefinition;

    public IDialogContext Create(IDialogDefinition dialog)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(dialog.Metadata))
            .WithCurrentPartId(new DialogPartIdentifierBuilder().WithValue("Empty"))
            .WithCurrentState(DialogState.Initial)
            .Build();
}
