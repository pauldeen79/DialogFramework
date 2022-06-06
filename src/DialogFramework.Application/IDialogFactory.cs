namespace DialogFramework.Application;

public interface IDialogFactory
{
    bool CanCreate(IDialogDefinition dialogDefinition);
    IDialog Create(IDialogDefinition dialogDefinition);
}
