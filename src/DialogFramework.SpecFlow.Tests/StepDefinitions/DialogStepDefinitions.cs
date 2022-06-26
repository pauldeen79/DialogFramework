namespace DialogFramework.SpecFlow.Tests.StepDefinitions;

[Binding]
public sealed class DialogStepDefinitions
{
    private Result<IDialog>? _lastResult;

    [Given(@"I start the '([^']*)' dialog")]
    public void GivenIStartTheDialog(IDialogDefinitionIdentifier id)
        => _lastResult = ApplicationEntrypoint.Instance.Start(id);

    [When(@"I answer the following results")]
    public void WhenIAnswerTheFollowingResults(IDialogPartResultAnswer[] answers)
        => _lastResult = ApplicationEntrypoint.Instance.Continue(GetCurrentDialog(), answers);

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
