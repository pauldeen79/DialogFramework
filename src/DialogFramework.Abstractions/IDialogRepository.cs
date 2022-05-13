namespace DialogFramework.Abstractions;

public interface IDialogRepository
{
    IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas();
    IDialog GetDialog(string id, string version);
}
