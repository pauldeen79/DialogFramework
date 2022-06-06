namespace DialogFramework.Application.TestData;

[ExcludeFromCodeCoverage]
public class TestDialogDefinitionRepository : IDialogDefinitionRepository
{
    private static readonly IDialogDefinition[] _dialogDefinitions = new[]
    {
        DialogDefinitionFixture.CreateBuilder().Build(),
        DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build(),
        TestFlowDialog.Create(),
        SimpleFormFlowDialog.Create(),
    };

    public IEnumerable<IDialogMetadata> GetAvailableDialogMetadatas()
        => _dialogDefinitions.Select(x => x.Metadata);

    public IDialogDefinition? GetDialogDefinition(IDialogDefinitionIdentifier identifier)
        => _dialogDefinitions.SingleOrDefault(x => Equals(x.Metadata.Id, identifier.Id)
                                                && Equals(x.Metadata.Version, identifier.Version));
}
