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

    private IDialog GetCurrentDialog()
    {
        if (_lastResult == null)
        {
            throw new InvalidOperationException("There is no current dialog. Did you start a dialog?");
        }
        return _lastResult!.GetValueOrThrow($"Last result was not successful. Details: {_lastResult}");
    }
}
