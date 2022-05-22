namespace DialogFramework.Core.Tests.Fixtures;

internal class TestDialogRepository : IDialogRepository
{
    private static readonly IDialog[] _dialogs = new IDialog[]
    {
        new TestFlowDialog(),
        new SimpleFormFlowDialog()
    };

    public IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas()
        => _dialogs.Select(x => x.Metadata);

    public IDialog? GetDialog(IDialogIdentifier identifier)
        => _dialogs.SingleOrDefault(x => x.Metadata.Id == identifier.Id && x.Metadata.Version == identifier.Version);
}
