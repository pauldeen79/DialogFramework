namespace DialogFramework.Tests.Fixtures;

internal class TestDialogRepository : IDialogRepository
{
    private static readonly IDialog[] _dialogs = new[]
    {
        TestFlowDialog.Create(),
        SimpleFormFlowDialog.Create()
    };

    public IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas()
        => _dialogs.Select(x => x.Metadata);

    public IDialog? GetDialog(IDialogIdentifier identifier)
        => _dialogs.SingleOrDefault(x => x.Metadata.Id == identifier.Id && x.Metadata.Version == identifier.Version);
}
