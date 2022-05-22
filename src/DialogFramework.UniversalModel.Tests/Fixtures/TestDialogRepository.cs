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

        public IDialog? GetDialog(IDialogIdentifier identifier)
        {
            var simpleFormFlowDialog = SimpleFormFlowDialog.Create();
            if (simpleFormFlowDialog.Metadata.Id == identifier.Id && simpleFormFlowDialog.Metadata.Version == identifier.Version)
            {
                return simpleFormFlowDialog;
            }

            var testFlowDialog = TestFlowDialog.Create();
            if (testFlowDialog.Metadata.Id == identifier.Id && testFlowDialog.Metadata.Version == identifier.Version)
            {
                return testFlowDialog;
            }

            return default;
        }
    }
}
