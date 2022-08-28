namespace DialogFramework.Application.Tests.RequestHandlers;

public abstract class RequestHandlerTestBase
{
    protected Mock<ILogger> LoggerMock { get; private set; }
    protected Mock<IDialogDefinitionProvider> ProviderMock { get; private set; }
    protected Mock<IConditionEvaluator> ConditionEvaluatorMock { get; private set; }

    protected RequestHandlerTestBase()
    {
        LoggerMock = new Mock<ILogger>();
        ProviderMock = new Mock<IDialogDefinitionProvider>();
        ConditionEvaluatorMock = new Mock<IConditionEvaluator>();

        ConditionEvaluatorMock.Setup(x => x.Evaluate(It.IsAny<object?>(), It.IsAny<IEnumerable<ICondition>>()))
                              .Returns(Result<bool>.Success(false));
    }

    protected static string Id => Guid.NewGuid().ToString();

    protected void AssertLogging(string title, string? exceptionMessage)
    {
        if (exceptionMessage != null)
        {
            LoggerMock.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, @type) => @object != null && @object.ToString() == title && @type != null && @type.Name == "FormattedLogValues"),
                    It.Is<InvalidOperationException>(ex => ex.Message == exceptionMessage),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce());
        }
        else
        {
            LoggerMock.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, @type) => @object != null && @object.ToString() == title && @type != null && @type.Name == "FormattedLogValues"),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce());
        }
    }
}
