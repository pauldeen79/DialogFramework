namespace DialogFramework.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void All_Dependencies_Can_Be_Resolved()
    {
        // Arrange
        var collection = new ServiceCollection();

        // Act
        var action = new Action(() => _ = collection.AddDialogFramework()
            .AddSingleton(new Mock<IDialogRepository>().Object)
            .AddSingleton(new Mock<ILogger>().Object)
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true }));

        // Assert
        action.Should().NotThrow();
    }
}
