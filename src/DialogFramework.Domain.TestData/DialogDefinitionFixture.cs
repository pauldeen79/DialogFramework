namespace DialogFramework.Domain.TestData;

[ExcludeFromCodeCoverage]
public static class DialogDefinitionFixture
{
    public static DialogDefinitionBuilder CreateBuilder()
        => CreateBuilderBase()
            .WithMetadata(DialogMetadataFixture.CreateBuilder().WithId("DialogDefinitionFixture"))
            .AddParts
            (
                QuestionDialogPartFixture.CreateBuilder(),
                new MessageDialogPartBuilder()
                    .WithId(new DialogPartIdentifierBuilder()
                    .WithValue("Message"))
                    .WithHeading("Message")
                    .WithMessage("This is a message")
                    .WithGroup(DialogPartGroupFixture.CreateBuilder())
            )
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder());

    public static DialogDefinitionBuilder CreateHowDoYouFeelBuilder(bool addParts = true)
    {
        var group1 = new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder().WithValue("Part1")).WithTitle("Give information").WithNumber(1);
        var group2 = new DialogPartGroupBuilder().WithId(new DialogPartGroupIdentifierBuilder().WithValue("Part2")).WithTitle("Completed").WithNumber(2);
        var welcomePart = new MessageDialogPartBuilder().WithHeading("Welcome").WithId(new DialogPartIdentifierBuilder().WithValue("Welcome")).WithMessage("Welcome! I would like to answer a question").WithGroup(group1);
        var answerGreat = new DialogPartResultAnswerDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Great")).WithTitle("I feel great, thank you!");
        var answerOkay = new DialogPartResultAnswerDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Okay")).WithTitle("I feel kind of okay");
        var answerTerrible = new DialogPartResultAnswerDefinitionBuilder().WithId(new DialogPartResultIdentifierBuilder().WithValue("Terrible")).WithTitle("I feel terrible, don't want to talk about it");
        var questionPart = new QuestionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Question1"))
            .WithHeading("How do you feel")
            .WithTitle("Please tell us how you feel")
            .WithGroup(group1)
            .AddAnswers(answerGreat, answerOkay, answerTerrible)
            .AddValidators(new QuestionDialogPartValidatorBuilder(new SingleRequiredQuestionDialogPartValidator()));
        var messagePart = new MessageDialogPartBuilder().WithId(new DialogPartIdentifierBuilder().WithValue("Message")).WithHeading("Message").WithMessage("I'm sorry to hear that. Let us know if we can do something to help you.").WithGroup(group1);
        var decisionPart = new DecisionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Decision"))
            .AddDecisions
            (
                new DecisionBuilder()
                    .AddConditions(new ConditionBuilder()
                        .WithLeftExpression(new GetDialogPartResultIdsByPartExpressionBuilder().WithDialogPartId(questionPart.Id))
                        .WithOperator(Operator.Contains)
                        .WithRightExpression(new ConstantExpressionBuilder().WithValue(answerTerrible.Id)))
                    .WithNextPartId(messagePart.Id)
            ).WithDefaultNextPartId(CreateBuilderBase().CompletedPart.Id);

        var parts = new IDialogPartBuilder[]
        {
            welcomePart,
            questionPart,
            decisionPart,
            messagePart
        }.Where(_ => addParts);
        return CreateBuilderBase()
            .WithMetadata(DialogMetadataFixture.CreateBuilder().WithId("HowDoYouFeel"))
            .AddParts(parts)
            .AddPartGroups(group1, group2);
    }

    public static DialogDefinitionBuilder CreateBuilderBase()
        => new DialogDefinitionBuilder()
            .WithMetadata(DialogMetadataFixture.CreateBuilder().WithId("DialogDefinitionFixtureBase"))
            .WithAbortedPart(new AbortedDialogPartBuilder()
                .WithId(new DialogPartIdentifierBuilder().WithValue("Abort"))
                .WithMessage("The dialog is aborted. You can come back any time to start the application again."))
            .WithErrorPart(new ErrorDialogPartBuilder()
                .WithId(new DialogPartIdentifierBuilder().WithValue("Error"))
                .WithErrorMessage("Something went wrong. Please try again, or contact us in case the problem persists."))
            .WithCompletedPart(new CompletedDialogPartBuilder()
                .WithId(new DialogPartIdentifierBuilder().WithValue("Completed"))
                .WithHeading("Completed")
                .WithMessage("Thank you for using this application. Please come back soon!")
                .WithGroup(DialogPartGroupFixture.CreateCompletedGroupBuilder()));

    public static IDialogDefinition CreateSingleStepDefinitionBuilder()
    {
        var welcomePart = new MessageDialogPartBuilder()
            .WithMessage("Welcome! I would like to answer a question")
            .WithGroup(DialogPartGroupFixture.CreateBuilder())
            .WithId(new DialogPartIdentifierBuilder().WithValue("Welcome"))
            .WithHeading("Welcome");
        var navigationPart = new NavigationDialogPartBuilder().WithId(new DialogPartIdentifierBuilder().WithValue("Navigate")).WithNavigateToId(welcomePart.Id);
        var dialogDefinition = CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Test dialog")
                .WithId("Test")
                .WithVersion("1.0.0"))
            .AddParts(navigationPart, welcomePart)
            .Build();
        return dialogDefinition;
    }

    public static IDialogDefinition CreateDialogDefinitionWithDecisionPartThatReturnsErrorDialogPart()
    {
        var decisionPart = new DecisionDialogPartBuilder()
            .WithId(new DialogPartIdentifierBuilder().WithValue("Decision"))
            .WithDefaultNextPartId(new DialogPartIdentifierBuilder().WithValue("Error"));
        var dialogDefinition = CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Test dialog")
                .WithId("Test")
                .WithVersion("1.0.0"))
            .AddParts(decisionPart)
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder())
            .Build();
        return dialogDefinition;
    }

    public static IDialogDefinition CreateFirstDialogDefinition(IDialogDefinition dialogDefinition2, bool addWelcomePart)
        => CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 1")
                .WithId("Dialog1")
                .WithVersion("1.0.0"))
            .AddParts(addWelcomePart
                ? new[]
                {
                    new MessageDialogPartBuilder()
                        .WithMessage("Welcome! I would like to answer a question")
                        .WithGroup(DialogPartGroupFixture.CreateBuilder())
                        .WithId(new DialogPartIdentifierBuilder().WithValue("Welcome"))
                        .WithHeading("Welcome")
                }
                : Enumerable.Empty<IDialogPartBuilder>())
            .AddParts(new RedirectDialogPartBuilder()
                .WithRedirectDialogMetadata(new DialogMetadataBuilder(dialogDefinition2.Metadata))
                .WithId(new DialogPartIdentifierBuilder().WithValue("Redirect")))
            .Build();

    public static IDialogDefinition CreateSecondDialogDefinition()
        => CreateBuilderBase()
            .WithMetadata(new DialogMetadataBuilder()
                .WithFriendlyName("Dialog 2")
                .WithId("Dialog2")
                .WithVersion("1.0.0"))
            .AddParts(new MessageDialogPartBuilder()
                .WithMessage("Welcome! I would like to answer a question")
                .WithGroup(DialogPartGroupFixture.CreateBuilder())
                .WithId(new DialogPartIdentifierBuilder().WithValue("Welcome"))
                .WithHeading("Welcome"))
            .AddPartGroups(DialogPartGroupFixture.CreateBuilder()).Build();

    public static (Mock<IDialogDefinition> DialogMock,
                   Mock<IAbortedDialogPart> AbortPartMock,
                   Mock<ICompletedDialogPart> CompletedPartMock,
                   Mock<IErrorDialogPart> ErrorPartMock,
                   Mock<IMessageDialogPart> MessagePartMock) CreateNavigatableDialogDefinition()
    {
        var mock = new Mock<IDialogDefinition>();
        var abortedPartMock = new Mock<IAbortedDialogPart>();
        var completedPartMock = new Mock<ICompletedDialogPart>();
        var errorPartMock = new Mock<IErrorDialogPart>();
        var messagePartMock = new Mock<IMessageDialogPart>();
        abortedPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifierBuilder().WithValue("Abort").Build());
        abortedPartMock.Setup(x => x.GetState()).Returns(DialogState.Aborted);
        completedPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifierBuilder().WithValue("Completed").Build());
        completedPartMock.Setup(x => x.GetState()).Returns(DialogState.Completed);
        errorPartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifierBuilder().WithValue("Error").Build());
        errorPartMock.Setup(x => x.GetState()).Returns(DialogState.ErrorOccured);
        messagePartMock.SetupGet(x => x.Id).Returns(new DialogPartIdentifierBuilder().WithValue("Message").Build());
        messagePartMock.Setup(x => x.GetState()).Returns(DialogState.InProgress);
        mock.SetupGet(x => x.AbortedPart).Returns(abortedPartMock.Object);
        mock.SetupGet(x => x.CompletedPart).Returns(completedPartMock.Object);
        mock.SetupGet(x => x.ErrorPart).Returns(errorPartMock.Object);
        mock.SetupGet(x => x.Metadata).Returns(new DialogMetadataBuilder().WithFriendlyName("test").WithId("test").WithVersion("1.0.0").Build());
        mock.SetupGet(x => x.PartGroups).Returns(new List<IDialogPartGroup>());
        mock.SetupGet(x => x.Parts).Returns(new List<IDialogPart>(new[] { messagePartMock.Object }));
        mock.Setup(x => x.GetPartById(It.IsAny<IDialogPartIdentifier>()))
            .Returns<IDialogPartIdentifier>(id => id.Value switch
            {
                "Abort" => Result<IDialogPart>.Success(abortedPartMock.Object),
                "Completed" => Result<IDialogPart>.Success(completedPartMock.Object),
                "Error" => Result<IDialogPart>.Success(errorPartMock.Object),
                "Message" => Result<IDialogPart>.Success(messagePartMock.Object),
                _ => Result<IDialogPart>.NotFound()
            });
        mock.Setup(x => x.GetNextPart(It.IsAny<IDialog>(), It.IsAny<IEnumerable<IDialogPartResultAnswer>>()))
            .Returns(Result<IDialogPart>.Success(completedPartMock.Object));
        mock.Setup(x => x.GetFirstPart())
            .Returns(Result<IDialogPart>.Success(messagePartMock.Object));
        mock.Setup(x => x.CanNavigateTo(It.IsAny<IDialogPartIdentifier>(), It.IsAny<IDialogPartIdentifier>(), It.IsAny<IEnumerable<IDialogPartResult>>()))
            .Returns(Result.Success());

        return (mock, abortedPartMock, completedPartMock, errorPartMock, messagePartMock);
    }
}
