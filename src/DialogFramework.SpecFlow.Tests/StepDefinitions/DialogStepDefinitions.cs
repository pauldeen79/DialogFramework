namespace DialogFramework.SpecFlow.Tests.StepDefinitions;

[Binding]
public sealed class DialogStepDefinitions
{
    private Result<IDialog>? _lastResult;

    [Given(@"I start the '([^']*)' dialog")]
    public void GivenIStartTheDialog(IDialogDefinitionIdentifier id)
        => _lastResult = ApplicationEntrypoint.DialogApplicationService.Start(id);

    [When(@"I answer the following results for the current dialog part")]
    public void WhenIAnswerTheFollowingResultsForTheCurrentDialogPart(Table table)
        => AnswerQuestionsFor(table);

    [Then("the current state should be (.*)")]
    public void ThenTheCurrentStateShouldBe(string result)
        => GetCurrentDialog().CurrentState.ToString().Should().Be(result);

    [When(@"I answer '([^']*)' for result '([^']*)'")]
    public void WhenIAnswerForResult(string value, string result)
        => Answer(result, value);

    [When(@"I answer '([^']*)' for the current result")]
    public void WhenIAnswerForTheCurrentResult(string value)
        => Answer(GetFirstResultIdOfCurrentPart(), value);

    [When(@"I answer No for result '([^']*)'")]
    public void WhenIAnswerNoForResult(string result)
        => Answer(result, false);

    [When(@"I answer Yes for result '([^']*)'")]
    public void WhenIAnswerYesForResult(string result)
        => Answer(result, true);

    [When(@"I answer No for the current result")]
    public void WhenIAnswerNoForTheCurrentResult()
        => Answer(GetFirstResultIdOfCurrentPart(), false);

    [When(@"I answer Yes for the current result")]
    public void WhenIAnswerYesForTheCurrentResult()
        => Answer(GetFirstResultIdOfCurrentPart(), true);

    private void AnswerQuestionsFor(Table table)
    {
        var results = table.CreateSet<(string Result, string Value)>()
            .Select(x => new DialogPartResultAnswerBuilder()
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(x.Result))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(x.Value))
                .Build());
        _lastResult = ApplicationEntrypoint.DialogApplicationService.Continue(GetCurrentDialog(), results);
    }

    private IDialog GetCurrentDialog()
    {
        if (_lastResult == null)
        {
            throw new InvalidOperationException("There is no current dialog. Did you start a dialog?");
        }
        return _lastResult!.GetValueOrThrow($"Last result was not successful. Details: {_lastResult}");
    }
    
    private void Answer(string result, object? value)
    {
        var results = new[]
        {
            new DialogPartResultAnswerBuilder()
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(result))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(value))
                .Build()
        };
        _lastResult = ApplicationEntrypoint.DialogApplicationService.Continue(GetCurrentDialog(), results);
    }

    private string GetFirstResultIdOfCurrentPart()
    {
        var dialog = GetCurrentDialog();
        var currentDialogId = dialog.CurrentDialogIdentifier;
        var currentPartId = dialog.CurrentPartId;
        var definition = ApplicationEntrypoint.DialogDefinitionRepository.GetDialogDefinition(currentDialogId) ?? throw new InvalidOperationException("Dialog definition could not be retrieved");
        var part = definition.GetPartById(currentPartId).GetValueOrThrow($"Could not get current part, Id is {currentPartId}");
        var questionDialogPart = part as IQuestionDialogPart ?? throw new InvalidOperationException($"Current part with Id [{currentPartId}] is not a question type, but {part.GetType().FullName}");
        return questionDialogPart.Results.First().Id.Value;
    }
}
