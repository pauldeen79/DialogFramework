namespace DialogFramework.SpecFlow.Tests.StepDefinitions;

[Binding]
public sealed class DialogStepDefinitions : IDisposable
{
    private readonly Mock<ILogger> _loggerMock;
    private readonly ServiceProvider _provider;
    private readonly IDialogApplicationService _dialogApplicationService;
    private Result<IDialog>? _lastResult;
    
    public DialogStepDefinitions()
    {
        _loggerMock = new Mock<ILogger>();
        _provider = new ServiceCollection()
            .AddDialogFramework()
            .AddSingleton<IDialogDefinitionRepository, TestDialogDefinitionRepository>()
            .AddSingleton(_loggerMock.Object)
            .BuildServiceProvider();
        _dialogApplicationService = _provider.GetRequiredService<IDialogApplicationService>();
    }

    [Given(@"I start the '([^']*)' dialog")]
    public void GivenIStartTheDialog(string id)
    {
        var identifier = new DialogDefinitionIdentifierBuilder().WithId(id).WithVersion("1.0.0").Build();
        _lastResult = _dialogApplicationService.Start(identifier);
    }

    [When(@"I answer the following questions for the '([^']*)' dialog part")]
    public void WhenIAnswerTheFollowingQuestionsForTheDialogPart(IDialogPartIdentifier dialogPartId, Table table)
        => AnswerQuestionsFor(dialogPartId, table);

    [When(@"I answer the following questions for the current dialog part")]
    public void WhenIAnswerTheFollowingQuestionsForTheCurrentDialogPart(Table table)
        => AnswerQuestionsFor(_lastResult!.GetValueOrThrow().CurrentPartId, table);

    [Then("the current state should be (.*)")]
    public void ThenTheCurrentStateShouldBe(string result)
    {
        EnsureDialog();
        _lastResult!.GetValueOrThrow(BuildErrorMessage(_lastResult)).CurrentState.ToString().Should().Be(result);
    }

    [When(@"I answer '([^']*)' for result '([^']*)'")]
    public void WhenIAnswerForResult(string value, string result)
        => Answer(result, value, ResultValueType.Text);

    [When(@"I answer No for result '([^']*)'")]
    public void WhenIAnswerNoForResult(string result) => AnwerBoolean(result, false);

    [When(@"I answer Yes for result '([^']*)'")]
    public void WhenIAnswerYesForResult(string result) => AnwerBoolean(result, true);

    public void Dispose() => _provider.Dispose();

    private void AnswerQuestionsFor(IDialogPartIdentifier dialogPartId, Table table)
    {
        EnsureDialog();
        var results = table.CreateSet<(string Result, ResultValueType ResultValueType, string Value)>()
            .Select(x => new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(dialogPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(x.Result))
                .WithValue(new DialogPartResultValueBuilder().WithResultValueType(x.ResultValueType).WithValue(ConvertValue(x.Value, x.ResultValueType)))
                .Build());
        _lastResult = _dialogApplicationService.Continue(_lastResult!.GetValueOrThrow(BuildErrorMessage(_lastResult)), results);
    }

    private void EnsureDialog()
        => _lastResult.Should().NotBeNull(because: "You should have started a dialog");

    private void AnwerBoolean(string result, bool value)
        => Answer(result, value, ResultValueType.YesNo);

    private void Answer(string result, object? value, ResultValueType type)
    {
        EnsureDialog();
        var results = new[]
        {
            new DialogPartResultBuilder()
                .WithDialogPartId(new DialogPartIdentifierBuilder(_lastResult!.GetValueOrThrow(BuildErrorMessage(_lastResult)).CurrentPartId))
                .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(result))
                .WithValue(new DialogPartResultValueBuilder().WithResultValueType(type).WithValue(value))
                .Build()
        };
        _lastResult = _dialogApplicationService.Continue(_lastResult!.GetValueOrThrow(BuildErrorMessage(_lastResult)), results);
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

    private static string BuildErrorMessage(Result<IDialog> result)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Status of last result was: {result.Status}");
        if (!string.IsNullOrEmpty(result.ErrorMessage))
        {
            sb.AppendLine($"Error message was: {result.ErrorMessage}");
        }
        if (result.ValidationErrors.Any())
        {
            var validationMessages = string.Join(Environment.NewLine, result.ValidationErrors.Select(x => $"{x.ErrorMessage} ({string.Join(", ", x.MemberNames)})"));
            sb.AppendLine($"Validation errors were:{Environment.NewLine}{validationMessages}");
        }
        return sb.ToString();
    }
}
