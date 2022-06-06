namespace DialogFramework.Domain;

public interface IDialogDefinitionRepository
{
    IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas();
    IDialogDefinition? GetDialog(IDialogIdentifier identifier);
}
