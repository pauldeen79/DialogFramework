namespace DialogFramework.Application;

public interface IDialogContextFactory
{
    bool CanCreate(IDialogDefinition dialog);
    IDialogContext Create(IDialogDefinition dialog);
}
