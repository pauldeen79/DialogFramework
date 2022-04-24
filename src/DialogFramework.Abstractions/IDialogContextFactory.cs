namespace DialogFramework.Abstractions;

public interface IDialogContextFactory
{
    bool CanCreate(IDialog dialog);
    IDialogContext Create(IDialog dialog);
}
