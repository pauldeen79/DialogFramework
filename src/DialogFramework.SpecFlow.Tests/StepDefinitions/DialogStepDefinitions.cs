namespace DialogFramework.SpecFlow.Tests.StepDefinitions;

[Binding]
public sealed class DialogStepDefinitions
{
    private Result<IDialog>? _lastResult;

    [Given(@"I start the '([^']*)' dialog")]
    public void GivenIStartTheDialog(IDialogDefinitionIdentifier id)
        => _lastResult = ApplicationEntrypoint.Instance.Start(id);

    [When(@"I answer the following results for the current dialog part")]
    public void WhenIAnswerTheFollowingResultsForTheCurrentDialogPart(DialogPartResultAnswerBuilder[] answers)
        => _lastResult = ApplicationEntrypoint.Instance.Continue(GetCurrentDialog(), answers.Select(x => x.Build()));

    [Then("the current state should be (.*)")]
    public void ThenTheCurrentStateShouldBe(string result)
        => GetCurrentDialog().CurrentState.ToString().Should().Be(result);

    [When(@"I answer '([^']*)' for result '([^']*)'")]
    public void WhenIAnswerForResult(string value, string result)
        => Answer(result, value);

    [When(@"I answer No for result '([^']*)'")]
    public void WhenIAnswerNoForResult(string result)
        => Answer(result, false);

    [When(@"I answer Yes for result '([^']*)'")]
    public void WhenIAnswerYesForResult(string result)
        => Answer(result, true);

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
        _lastResult = ApplicationEntrypoint.Instance.Continue(GetCurrentDialog(), results);
    }
}
