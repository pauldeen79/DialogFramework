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

    public IDialog GetDialog(IDialogMetadata metadata)
        => _dialogs.Single(x => x.Metadata.Id == metadata.Id && x.Metadata.Version == metadata.Version);
}
