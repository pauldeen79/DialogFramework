using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;

namespace DialogFramework.UniversalModel.Tests.Fixtures
{
    public class TestDialogRepository : IDialogRepository
    {
        public IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas()
        {
            yield return SimpleFormFlowDialog.Create().Metadata;
            yield return TestFlowDialog.Create().Metadata;
        }

        public IDialog GetDialog(string id, string version)
        {
            var simpleFormFlowDialog = SimpleFormFlowDialog.Create();
            if (simpleFormFlowDialog.Metadata.Id == id && simpleFormFlowDialog.Metadata.Version == version)
            {
                return simpleFormFlowDialog;
            }

            var testFlowDialog = TestFlowDialog.Create();
            if (testFlowDialog.Metadata.Id == id && testFlowDialog.Metadata.Version == version)
            {
                return testFlowDialog;
            }

            throw new NotSupportedException($"Could not create repository with id [{id}], version [{version}]");
        }
    }
}
