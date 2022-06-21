namespace DialogFramework.SpecFlow.Tests.StepDefinitions;

[Binding]
public sealed class DialogStepDefinitions
{
    private Result<IDialog>? _lastResult;

    [Given(@"I start the '([^']*)' dialog")]
    public void GivenIStartTheDialog(IDialogDefinitionIdentifier id)
        => _lastResult = ApplicationEntrypoint.Instance.Start(id);

    [When(@"I answer the following results")]
    public void WhenIAnswerTheFollowingResults(DialogPartResultAnswerBuilder[] answers)
        => _lastResult = ApplicationEntrypoint.Instance.Continue(GetCurrentDialog(), answers.Select(x => x.Build()));

    [Then("the current state should be (.*)")]
    public void ThenTheCurrentStateShouldBe(DialogState state)
        => GetCurrentDialog().CurrentState.Should().Be(state);

    [When(@"I answer '([^']*)' for result '([^']*)'")]
    public void WhenIAnswerForResult(string value, IDialogPartResultIdentifier result)
        => Answer(result, value);

    [When(@"I answer No for result '([^']*)'")]
    public void WhenIAnswerNoForResult(IDialogPartResultIdentifier result)
        => Answer(result, false);

    [When(@"I answer Yes for result '([^']*)'")]
    public void WhenIAnswerYesForResult(IDialogPartResultIdentifier result)
        => Answer(result, true);

    private IDialog GetCurrentDialog()
    {
        if (_lastResult == null)
        {
            throw new InvalidOperationException("There is no current dialog. Did you start a dialog?");
        }
        return _lastResult!.GetValueOrThrow($"Last result was not successful. Details: {_lastResult}");
    }
    
    private void Answer(IDialogPartResultIdentifier resultId, object? value)
    {
        var results = new[]
        {
            new DialogPartResultAnswerBuilder()
                .WithResultId(new DialogPartResultIdentifierBuilder(resultId))
                .WithValue(new DialogPartResultValueAnswerBuilder().WithValue(value))
                .Build()
        };
        _lastResult = ApplicationEntrypoint.Instance.Continue(GetCurrentDialog(), results);
    }
}
