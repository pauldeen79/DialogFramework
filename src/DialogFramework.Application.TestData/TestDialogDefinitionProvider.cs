namespace DialogFramework.Application.TestData;

[ExcludeFromCodeCoverage]
public class TestDialogDefinitionProvider : IDialogDefinitionProvider
{
    private static readonly IDialogDefinition[] _dialogDefinitions = new[]
    {
        DialogDefinitionFixture.CreateBuilder().Build(),
        DialogDefinitionFixture.CreateHowDoYouFeelBuilder().Build(),
        TestFlowDialog.Create(),
        SimpleFormFlowDialog.Create(),
    };

    public Result<IDialogDefinition> GetDialogDefinition(IDialogDefinitionIdentifier identifier)
        => Result.FromInstance(_dialogDefinitions.SingleOrDefault(x => Equals(x.Metadata.Id, identifier.Id)
                               && Equals(x.Metadata.Version, identifier.Version)));
}
