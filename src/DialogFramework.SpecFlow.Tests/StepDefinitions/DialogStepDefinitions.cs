namespace DialogFramework.SpecFlow.Tests.StepDefinitions;

[Binding]
public sealed class DialogStepDefinitions
{
    private Result<IDialog>? _lastResult;

    [Given(@"I start the '([^']*)' dialog")]
    public async Task GivenIStartTheDialog(IDialogDefinitionIdentifier id)
        => _lastResult = await ApplicationEntrypoint.StartHandler.Handle(new StartRequest(id), CancellationToken.None);

    [When(@"I answer the following results")]
    public async Task WhenIAnswerTheFollowingResults(IDialogPartResultAnswer[] answers)
        => _lastResult = await ApplicationEntrypoint.ContinueHandler.Handle(new ContinueRequest(GetCurrentDialog(), answers), CancellationToken.None);

    [Then(@"the dialog should contain the content")]
    public void ValidateResponseContent(Table table)
        => table.CompareToInstance(GetCurrentDialog());

    private IDialog GetCurrentDialog()
    {
        if (_lastResult == null)
        {
            throw new InvalidOperationException("There is no current dialog. Did you start a dialog?");
        }
        return _lastResult!.GetValueOrThrow($"Last result was not successful. Details: {_lastResult}");
    }
}
