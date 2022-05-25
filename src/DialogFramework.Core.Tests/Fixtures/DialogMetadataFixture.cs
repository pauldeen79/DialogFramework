namespace DialogFramework.Core.Tests.Fixtures;

internal static class DialogMetadataFixture
{
    internal static DialogMetadataBuilder CreateBuilder()
        => new DialogMetadataBuilder().WithId("Test").WithFriendlyName("Test dialog").WithVersion("1.0.0");
}
