namespace DialogFramework.SpecFlow.Tests.StepDefinitions;

[Binding]
public sealed class DialogStepDefinitions
{
    private Result<IDialog>? _lastResult;

    [Given(@"I start the '([^']*)' dialog")]
    public void GivenIStartTheDialog(IDialogDefinitionIdentifier id)
        => _lastResult = ApplicationEntrypoint.DialogApplicationService.Start(id);

    [When(@"I answer the following results for the '([^']*)' dialog part")]
    public void WhenIAnswerTheFollowingResultsForTheDialogPart(IDialogPartIdentifier dialogPartId, Table table)
        => AnswerQuestionsFor(dialogPartId, table);

    [When(@"I answer the following results for the current dialog part")]
    public void WhenIAnswerTheFollowingResultsForTheCurrentDialogPart(Table table)
        => AnswerQuestionsFor(GetCurrentDialog().CurrentPartId, table);

    [Then("the current state should be (.*)")]
    public void ThenTheCurrentStateShouldBe(string result)
        => GetCurrentDialog().CurrentState.ToString().Should().Be(result);

    [When(@"I answer '([^']*)' for result '([^']*)'")]
    public void WhenIAnswerForResult(string value, string result)
        => Answer(result, value, ResultValueType.Text);

    [When(@"I answer '([^']*)' for the current result")]
    public void WhenIAnswerForTheCurrentResult(string value)
        => Answer(GetCurrentResultId(), value, ResultValueType.Text);

    [When(@"I answer No for result '([^']*)'")]
    public void WhenIAnswerNoForResult(string result)
        => AnwerBoolean(result, false);

    [When(@"I answer Yes for result '([^']*)'")]
    public void WhenIAnswerYesForResult(string result)
        => AnwerBoolean(result, true);

    [When(@"I answer No for the current result")]
    public void WhenIAnswerNoForTheCurrentResult()
        => AnwerBoolean(GetCurrentResultId(), false);

    [When(@"I answer Yes for the current result")]
    public void WhenIAnswerYesForTheCurrentResult()
        => AnwerBoolean(GetCurrentResultId(), true);

    private void AnswerQuestionsFor(IDialogPartIdentifier dialogPartId, Table table)
    {
        var results = table.CreateSet<(string Result, ResultValueType ResultValueType, string Value)>()
            .Select(x => new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialogPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(x.Result))
                .WithValue(new DialogPartResultValueBuilder().WithResultValueType(x.ResultValueType).WithValue(ConvertValue(x.Value, x.ResultValueType)))
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
    
    private void AnwerBoolean(string resultId, bool value)
        => Answer(resultId, value, ResultValueType.YesNo);

    private void Answer(string result, object? value, ResultValueType type)
    {
        var results = new[]
        {
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(GetCurrentDialog().CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(result))
                .WithValue(new DialogPartResultValueBuilder().WithResultValueType(type).WithValue(value))
                .Build()
        };
        _lastResult = ApplicationEntrypoint.DialogApplicationService.Continue(GetCurrentDialog(), results);
    }

    private string GetCurrentResultId()
    {
        var dialog = GetCurrentDialog();
        var currentDialogId = dialog.CurrentDialogIdentifier;
        var currentPartId = dialog.CurrentPartId;
        var definition = ApplicationEntrypoint.DialogDefinitionRepository.GetDialogDefinition(currentDialogId) ?? throw new InvalidOperationException("Dialog definition could not be retrieved");
        var part = definition.GetPartById(currentPartId).GetValueOrThrow($"Could not get current part, Id is {currentPartId}");
        var questionDialogPart = part as IQuestionDialogPart;
        if (questionDialogPart == null)
        {
            throw new InvalidOperationException($"Current part with Id [{currentPartId}] is not a question type, but {part.GetType().FullName}");
        }
        return questionDialogPart.Results.First().Id.Value;
    }

    private static object? ConvertValue(string value, ResultValueType resultValueType) => resultValueType switch
    {
        ResultValueType.None => null,
        ResultValueType.Text => value,
        ResultValueType.Number => Convert.ToDecimal(value, CultureInfo.InvariantCulture),
        ResultValueType.YesNo => Convert.ToBoolean(value, CultureInfo.InvariantCulture),
        ResultValueType.Date or ResultValueType.DateTime => Convert.ToDateTime(value, CultureInfo.InvariantCulture),
        _ => throw new NotSupportedException($"Unsupported result value type: {resultValueType}"),
    };
}
