using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;

namespace DialogFramework.UniversalModel.Tests.Fixtures
{
    public class TestDialogRepository : IDialogRepository
    {
        public IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas()
        {
            yield return SimpleFormFlowDialog.Create().Metadata;
        }

        public IDialog GetDialog(string id, string version)
        {
            var dialog = SimpleFormFlowDialog.Create();
            var metadata = dialog.Metadata;

            if (metadata.Id == id && metadata.Version == version)
            {
                return dialog;
            }

            throw new NotSupportedException($"Could not create repository with id [{id}], version [{version}]");
        }
    }
}
