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

    public Result<IDialogDefinition> GetDialogDefinition(IDialogDefinitionIdentifier id)
        => Result.FromInstance(_dialogDefinitions.SingleOrDefault(x => Equals(x.Metadata.Id, id.Id)
                               && Equals(x.Metadata.Version, id.Version)));
}
