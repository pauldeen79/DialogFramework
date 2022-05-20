using CodeGeneration.Tests.CodeGenerationProviders;

namespace CodeGeneration.Tests;

public class CodeGenerationTests
{
    private static readonly CodeGenerationSettings Settings = new CodeGenerationSettings
    (
        basePath: Path.Combine(Directory.GetCurrentDirectory(), @"../../../../"),
        generateMultipleFiles: true,
        dryRun: true
    );

    //// Bootstrap test that generates c# code for the model used in code generation :)
    //[Fact]
    //public void Can_Generate_Model_For_Abstractions()
    //{
    //    // Act & Assert
    //    Verify(GenerateCode.For<InterfacesModels>(Settings));
    //}

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

    //private static void Verify(GenerateCode generatedCode)
    //{
    //    if (Settings.DryRun)
    //    {
    //        // Act
    //        var actual = generatedCode.GenerationEnvironment.ToString();

    //        // Assert
    //        actual.NormalizeLineEndings().Should().NotBeNullOrEmpty().And.NotStartWith("Error:");
    //    }
    //}

    private static void Verify(MultipleContentBuilder multipleContentBuilder)
    {
        var actual = multipleContentBuilder.ToString();

        // Assert
        actual.NormalizeLineEndings().Should().NotBeNullOrEmpty();
    }
}
