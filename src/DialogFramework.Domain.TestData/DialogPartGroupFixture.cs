namespace DialogFramework.Domain.TestData;

[ExcludeFromCodeCoverage]
public static class DialogPartGroupFixture
{
    public static DialogPartGroupBuilder CreateBuilder()
        => new DialogPartGroupBuilder()
            .WithId(new DialogPartGroupIdentifierBuilder().WithValue("Group"))
            .WithTitle("Group")
            .WithNumber(1);

    public static DialogPartGroupBuilder CreateWelcomeGroupBuilder()
        => new DialogPartGroupBuilder()
            .WithId(new DialogPartGroupIdentifierBuilder().WithValue("Welcome"))
            .WithNumber(1)
            .WithTitle("Welcome");

    public static DialogPartGroupBuilder CreateGetInformationGroupBuider()
        => new DialogPartGroupBuilder()
            .WithId(new DialogPartGroupIdentifierBuilder().WithValue("Get information"))
            .WithNumber(2)
            .WithTitle("Get information");

    public static DialogPartGroupBuilder CreateCompletedGroupBuilder()
        => new DialogPartGroupBuilder()
            .WithId(new DialogPartGroupIdentifierBuilder().WithValue("Completed"))
            .WithNumber(3)
            .WithTitle("Completed");
}
