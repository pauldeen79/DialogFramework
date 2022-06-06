namespace DialogFramework.Application.TestData;

public class TestDialogDefinitionRepository : IDialogDefinitionRepository
{
    private static readonly IDialogDefinition[] _dialogs = new[]
    {
        DialogDefinitionFixture.CreateBuilder().Build(),
        DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build(),
        TestFlowDialog.Create(),
        SimpleFormFlowDialog.Create(),
    };

    public IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas()
        => _dialogs.Select(x => x.Metadata);

    public IDialogDefinition? GetDialog(IDialogIdentifier identifier)
        => _dialogs.SingleOrDefault(x => Equals(x.Metadata.Id, identifier.Id) && Equals(x.Metadata.Version, identifier.Version));
}
