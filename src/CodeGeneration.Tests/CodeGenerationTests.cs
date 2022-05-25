namespace CodeGeneration.Tests;

public class CodeGenerationTests
{
    private static readonly CodeGenerationSettings Settings = new CodeGenerationSettings
    (
        basePath: Path.Combine(Directory.GetCurrentDirectory(), @"../../../../"),
        generateMultipleFiles: true,
        dryRun: true
    );

    [Fact]
    public void Can_Generate_All_Classes_For_DialogFramework()
    {
        // Arrange
        var multipleContentBuilder = new MultipleContentBuilder(Settings.BasePath);

        // Act
        GenerateCode.For<CoreBuilders>(Settings, multipleContentBuilder);
        GenerateCode.For<CoreRecords>(Settings, multipleContentBuilder);
        GenerateCode.For<DomainModelBuilders>(Settings, multipleContentBuilder);
        GenerateCode.For<DomainModelRecords>(Settings, multipleContentBuilder);
        GenerateCode.For<DialogPartBuilders>(Settings, multipleContentBuilder);
        GenerateCode.For<DialogPartRecords>(Settings, multipleContentBuilder);

        // Assert
        Verify(multipleContentBuilder);
    }

    private static void Verify(MultipleContentBuilder multipleContentBuilder)
    {
        var actual = multipleContentBuilder.ToString();

        // Assert
        actual.NormalizeLineEndings().Should().NotBeNullOrEmpty();
    }
}
