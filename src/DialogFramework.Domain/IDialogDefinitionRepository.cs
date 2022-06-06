namespace DialogFramework.Domain;

public interface IDialogDefinitionRepository
{
    IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas();
    IDialogDefinition? GetDialogDefinition(IDialogDefinitionIdentifier identifier);
}
