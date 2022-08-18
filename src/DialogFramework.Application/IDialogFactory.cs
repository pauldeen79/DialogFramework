namespace DialogFramework.Application;

public interface IDialogFactory
{
    Result<IDialog> Create(IDialogDefinition definition,
                           IEnumerable<IDialogPartResult> results,
                           IEnumerable<IProperty> properties);
}
