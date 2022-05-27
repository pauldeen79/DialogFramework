namespace DialogFramework.Application;

public interface IDialogRepository
{
    IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas();
    IDialog? GetDialog(IDialogIdentifier identifier);
}
