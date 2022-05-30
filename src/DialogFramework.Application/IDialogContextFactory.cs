namespace DialogFramework.Application;

public interface IDialogContextFactory
{
    bool CanCreate(IDialog dialog);
    IDialogContext Create(IDialog dialog);
}
