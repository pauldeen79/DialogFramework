namespace DialogFramework.Domain.Tests.Fixtures;

internal static class DialogPartGroupFixture
{
    internal static DialogPartGroupBuilder CreateBuilder()
        => new DialogPartGroupBuilder()
            .WithId("Group")
            .WithTitle("Group")
            .WithNumber(1);
}
