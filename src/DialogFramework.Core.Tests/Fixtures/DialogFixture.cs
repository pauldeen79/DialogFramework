namespace DialogFramework.Core.Tests.Fixtures;

internal static class DialogFixture
{
    internal static Dialog CreateDialog(bool addParts = true)
    {
        var group1 = new DialogPartGroup("Part1", "Give information", 1);
        var group2 = new DialogPartGroup("Part2", "Completed", 2);
        var welcomePart = new MessageDialogPart("Welcome", "Welcome", "Welcome! I would like to answer a question", group1);
        var errorDialogPart = new ErrorDialogPart("Error", "Something went horribly wrong!", null);
        var abortedPart = new AbortedDialogPart("Abort", "Dialog has been aborted");
        var answerGreat = new DialogPartResultDefinition("Great", "I feel great, thank you!", ResultValueType.None);
        var answerOkay = new DialogPartResultDefinition("Okay", "I feel kind of okay", ResultValueType.None);
        var answerTerrible = new DialogPartResultDefinition("Terrible", "I feel terrible, don't want to talk about it", ResultValueType.None);
        var questionPart = new QuestionDialogPart("Question1", "How do you feel", "Please tell us how you feel", group1, new[] { answerGreat, answerOkay, answerTerrible }, Enumerable.Empty<IQuestionDialogPartValidator>());
        var messagePart = new MessageDialogPart("Message", "Message", "I'm sorry to hear that. Let us know if we can do something to help you.", group1);
        var completedPart = new CompletedDialogPart("Completed", "Completed", "Thank you for your input!", group2);
        var decisionPart = new DecisionDialogPartFixture
        (
            "Decision",
            (ctx, dialog) => ctx.GetDialogPartResultsByPart(questionPart).Single().ResultId == answerTerrible.Id
                ? messagePart.Id
                : completedPart.Id
        );
        var parts = new IDialogPart[]
        {
            welcomePart,
            questionPart,
            decisionPart,
            messagePart
        }.Where(_ => addParts);
        return new Dialog(
            new DialogMetadata(
                "Test",
                "Test dialog",
                "1.0.0",
                true),
            parts,
            errorDialogPart,
            abortedPart,
            completedPart,
            new[] { group1, group2 }
            );
    }
}
