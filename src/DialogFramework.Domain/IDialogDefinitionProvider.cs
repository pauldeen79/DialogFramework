namespace DialogFramework.Domain;

public interface IDialogDefinitionProvider
{
    Result<IDialogDefinition> GetDialogDefinition(IDialogDefinitionIdentifier identifier);
}
