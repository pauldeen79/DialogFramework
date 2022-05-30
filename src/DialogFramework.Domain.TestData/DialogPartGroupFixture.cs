namespace DialogFramework.Domain.TestData;

public static class DialogPartGroupFixture
{
    public static DialogPartGroupBuilder CreateBuilder()
        => new DialogPartGroupBuilder()
            .WithId(new DialogPartGroupIdentifierBuilder().WithValue("Group"))
            .WithTitle("Group")
            .WithNumber(1);
}
