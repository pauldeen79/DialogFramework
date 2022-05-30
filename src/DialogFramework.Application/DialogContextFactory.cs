namespace DialogFramework.Application;

public class DialogContextFactory : IDialogContextFactory
{
    public bool CanCreate(IDialog dialog) => dialog is Dialog;

    public IDialogContext Create(IDialog dialog)
        => new DialogContextBuilder()
            .WithId(new DialogContextIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogIdentifierBuilder(dialog.Metadata))
            .WithCurrentPartId(new DialogPartIdentifierBuilder().WithValue("Empty"))
            .WithCurrentState(DialogState.Initial)
            .Build();
}
