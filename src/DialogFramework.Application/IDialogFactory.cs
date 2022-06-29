namespace DialogFramework.Application;

public interface IDialogFactory
{
    Result<IDialog> Create(IDialogDefinition dialogDefinition,
                           IEnumerable<IDialogPartResult> dialogPartResults);
}
