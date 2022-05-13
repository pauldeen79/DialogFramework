namespace DialogFramework.Abstractions;

public interface IDialogRepository
{
    IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas();
    IDialog GetDialog(IDialogMetadata metadata);
}
