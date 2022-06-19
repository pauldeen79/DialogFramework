namespace DialogFramework.SpecFlow.Tests.StepDefinitions
{
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
        public void WhenIAnswerTheFollowingQuestionsForTheDialogPart(string dialogPartId, Table table)
        {
            _lastResult.Should().NotBeNull(because: "You should have started a dialog");
            var results = table.CreateSet<(string Result, string ResultValueType, string Value)>()
                .Select(x => new DialogPartResultBuilder()
                    .WithDialogPartId(new DialogPartIdentifierBuilder().WithValue(dialogPartId))
                    .WithResultId(new DialogPartResultIdentifierBuilder().WithValue(x.Result))
                    .WithValue(new DialogPartResultValueBuilder().WithResultValueType(Enum.Parse<ResultValueType>(x.ResultValueType)).WithValue(ConvertValue(x.Value, Enum.Parse<ResultValueType>(x.ResultValueType))))
                    .Build());
            _lastResult = _dialogApplicationService.Continue(_lastResult!.GetValueOrThrow(_lastResult.ErrorMessage ?? string.Empty), results);
        }

        [Then("the current state should be (.*)")]
        public void ThenTheResultShouldBe(string result)
        {
            _lastResult.Should().NotBeNull(because: "You should have answered questions");
            _lastResult!.Value.Should().NotBeNull(because: "You should have answered questions");
            _lastResult!.Value!.CurrentState.ToString().Should().Be(result);
        }

        public void Dispose() => _provider.Dispose();

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
}
