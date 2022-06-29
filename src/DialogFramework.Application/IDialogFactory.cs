namespace DialogFramework.Application;

public interface IDialogFactory
{
    bool CanCreate(IDialogDefinition dialogDefinition, IEnumerable<IDialogPartResult> dialogPartResults);
    IDialog Create(IDialogDefinition dialogDefinition, IEnumerable<IDialogPartResult> dialogPartResults);
}
