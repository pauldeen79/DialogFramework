namespace DialogFramework.CodeGeneration;

[ExcludeFromCodeCoverage]
internal static class Program
{
    private static void Main(string[] args)
    {
        // Setup code generation
        var currentDirectory = Directory.GetCurrentDirectory();
        var basePath = currentDirectory.EndsWith("DialogFramework")
            ? Path.Combine(currentDirectory, @"src/")
            : Path.Combine(currentDirectory, @"../../../../");
        var generateMultipleFiles = true;
        var dryRun = false;
        var multipleContentBuilder = new MultipleContentBuilder { BasePath = basePath };

        // Generate code
        var generationTypeNames = new[] { "Entities", "Builders", "Models", "BuilderFactory", "ModelFactory" };
        var generators = typeof(DialogFrameworkCSharpClassBase).Assembly.GetExportedTypes().Where(x => !x.IsAbstract && x.BaseType == typeof(DialogFrameworkCSharpClassBase)).ToArray();
        var generationTypes = generators.Where(x => x.Name.EndsWithAny(generationTypeNames));
        var scaffoldingTypes = generators.Where(x => !x.Name.EndsWithAny(generationTypeNames));
        _ = generationTypes.Select(x => (DialogFrameworkCSharpClassBase)Activator.CreateInstance(x)!).Select(x => GenerateCode.For(new(basePath, generateMultipleFiles, false, dryRun), multipleContentBuilder, x)).ToArray();
        _ = scaffoldingTypes.Select(x => (DialogFrameworkCSharpClassBase)Activator.CreateInstance(x)!).Select(x => GenerateCode.For(new(basePath, generateMultipleFiles, true, dryRun), multipleContentBuilder, x)).ToArray();

        var modelGenerators = typeof(DialogFrameworkModelClassBase).Assembly.GetExportedTypes().Where(x => !x.IsAbstract && x.BaseType == typeof(DialogFrameworkModelClassBase)).ToArray();
        var modelGenerationTypes = modelGenerators.Where(x => x.Name.EndsWithAny(generationTypeNames));
        _ = modelGenerationTypes.Select(x => (DialogFrameworkModelClassBase)Activator.CreateInstance(x)!).Select(x => GenerateCode.For(new(basePath, generateMultipleFiles, false, dryRun), multipleContentBuilder, x)).ToArray();

        // Log output to console
        if (string.IsNullOrEmpty(basePath))
        {
            Console.WriteLine(multipleContentBuilder.ToString());
        }
        else
        {
            Console.WriteLine($"Code generation completed, check the output in {basePath}");
            Console.WriteLine($"Generated files: {multipleContentBuilder.Contents.Count()}");
        }
    }
}
