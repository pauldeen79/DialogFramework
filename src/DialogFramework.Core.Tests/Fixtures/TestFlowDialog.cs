namespace DialogFramework.Core.Tests.Fixtures;

public record TestFlowDialog : Dialog
{
    private static readonly IDialogPartGroup WelcomeGroup = new DialogPartGroup("Welcome", "Welcome", 1);
    private static readonly IDialogPartGroup GetInformationGroup = new DialogPartGroup("Get information", "Get information", 2);
    private static readonly IDialogPartGroup CompletedGroup = new DialogPartGroup("Completed", "Completed", 3);
    private const string EmailPartId = "Email";

    public TestFlowDialog() : base(
        new DialogMetadata(nameof(TestFlowDialog), "Test flow dialog", "1.0.0", true),
        new IDialogPart[]
        {
            new MessageDialogPart("Welcome", "Welcome", "Welcome to the health advisor application. By answering questions, we can give you an advice how to improve your health. You can continue to start analyzing your health.", WelcomeGroup),
            new SingleRequiredQuestionDialogPart
            (
                "Age",
                "Age",
                "How old are you?",
                GetInformationGroup,
                new IDialogPartResultDefinition[]
                {
                    new DialogPartResultDefinition("<10", "0 to 9 years old", ResultValueType.None),
                    new DialogPartResultDefinition("10-19", "10 to 19 years old", ResultValueType.None),
                    new DialogPartResultDefinition("20-29", "20 to 29 years old", ResultValueType.None),
                    new DialogPartResultDefinition("30-39", "30 to 39 years old", ResultValueType.None),
                    new DialogPartResultDefinition("40-49", "40 to 49 years old", ResultValueType.None),
                    new DialogPartResultDefinition("50+", "Older than 50 years", ResultValueType.None),
                }
            ),
            new AgeDecisionDialogPart("AgeDecision"),
            new MessageDialogPart("TooYoung", "Completed", "Too bad, you are too young. We can't give advice on kids.", CompletedGroup),
            new StaticNavigationDialogPart("TooYoungNavigation", EmailPartId),
            new QuestionDialogPart
            (
                "SportsTypes",
                "Sports types",
                "What type of sports do you do?",
                GetInformationGroup,
                new IDialogPartResultDefinition[]
                {
                    new DialogPartResultDefinition("Bicycle", "Bicycle riding", ResultValueType.None),
                    new DialogPartResultDefinition("Soccer", "Socced", ResultValueType.None),
                    new DialogPartResultDefinition("Swimming", "Swimming", ResultValueType.None),
                    new DialogPartResultDefinition("Aerobics", "Aerobics", ResultValueType.None),
                    new DialogPartResultDefinition("Tennis", "Tennis", ResultValueType.None),
                    new DialogPartResultDefinition("Baseball", "Baseball", ResultValueType.None),
                    new DialogPartResultDefinition("Hockey", "Hockey", ResultValueType.None),
                    new DialogPartResultDefinition("Other", "Other sports (please specify)", ResultValueType.Text),
                }
            ),
            new SportsTypeDecisionDialogPart("SportsTypeDecision"),
            new MessageDialogPart("Healthy", "Healthy", "You're all good! Keep up the good work.", CompletedGroup),
            new StaticNavigationDialogPart("HealthyNavigation", EmailPartId),
            new MessageDialogPart("Unhealthy", "Unhealthy", "Our advice: It's time to do some sports, mate!", CompletedGroup),
            new StaticNavigationDialogPart("UnhealthyNavigation", EmailPartId),
            new SingleOptionalQuestionDialogPart(EmailPartId, "E-mail address", "Thank you for using this application. You can leave your e-mail address in case you have comments or questions.", CompletedGroup, new[] { new DialogPartResultDefinition("EmailAddress", "E-mail address", ResultValueType.Text) })
        },
        new ErrorDialogPart("Error", "Something went wrong. Please try again, or contact us in case the problem persists.", null),
        new AbortedDialogPart("Aborted", "The dialog is aborted. You can come back any time to start the application again."),
        new CompletedDialogPart("Completed", "Completed", "Thank you for using this application. Please come back soon!", CompletedGroup),
        new[] { WelcomeGroup, GetInformationGroup, CompletedGroup }
        )
    {
    }
}

/*
Test flow

1. Welcome! (Message)
   Welcome to the health advisor application

2. How old are you? (Question, single answer)
   A. 0-10 years old
   B. 10-20 years old
   C. 20-30 years old
   D. 30-40 yeard old
   E. 40-50 years old
   F. older than 50 years

3. Decision: A -> 4. TooYoung
             B -> 5. SportsType
			 
4. TooYoung (Message)
   Too bad! This application is not built for your age. You are too young.
   
Redirect/forward to mail
 
5. SportsType
   What type of sports do you do? (Question, multiple answers)
   A. Bicycle
   B. Soccer
   C. Swimming
   D. Aerobics
   E. Other: ... (free text)

6. Decision: none     -> 7. Unhealthy
             not none -> 8. Healthy

7. Unhealthy (Message)
   Our advice is to do some sports.

Redirect/forward to mail

8. Healthy (Message)
   You're all good!

Redirect/forward to mail

9. Mail (Question, single answer)
   If you want to, you could provide your e-mail address so we can contact you.
   A. e-mail adress (text, validate for e-mail address)

10. Thank you!
*/
