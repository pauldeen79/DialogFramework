namespace DialogFramework.Domain.TestData;

[ExcludeFromCodeCoverage]
public static class DialogMetadataFixture
{
    public static DialogMetadataBuilder CreateBuilder()
        => new DialogMetadataBuilder()
            .WithId("Test")
            .WithFriendlyName("Test dialog")
            .WithVersion("1.0.0");
}
