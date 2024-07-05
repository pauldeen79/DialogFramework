namespace DialogFramework.Domain.TestData;

public static class TestDialogDefinitionFactory
{
    public static DialogDefinition CreateEmpty() => new DialogDefinitionBuilder()
        .WithId("MyDialogDefinition")
        .WithName("My dialog definition")
        .Build();

    public static DialogDefinition CreateDialogWithRequiredQuestion() => new DialogDefinitionBuilder()
        .WithId("MyDialogWithRequiredQuestion")
        .WithName("My dialog with required question")
        .AddSections(new DialogPartSectionBuilder()
            .WithId("MySection")
            .WithName("My section")
            .AddParts(
                new LabelDialogPartBuilder().WithId("Label").WithTitle("Title"),
                new SingleOpenQuestionDialogPartBuilder().WithId("Question").WithTitle("What's your name?")
                    .AddValidationRules(new RequiredValidationRuleBuilder())
            )
        )
        .Build();

    public static DialogDefinition CreateDialogWithCustomDialogPart(DialogPart part) => new(
        "MyDialogWithRequiredQuestion",
        "My dialog with required question",
        new Version(1, 0, 0),
        [new DialogPartSection("Id", null, "Name", [part])]);
}
