namespace DialogFramework.Application;

public interface IDialogFactory
{
    bool CanCreate(IDialogDefinition dialog);
    IDialog Create(IDialogDefinition dialog);
}
