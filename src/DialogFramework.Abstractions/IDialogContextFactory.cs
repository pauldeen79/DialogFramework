namespace DialogFramework.Abstractions;

public interface IDialogContextFactory
{
    IDialogContext Create(IDialog dialog);
}
