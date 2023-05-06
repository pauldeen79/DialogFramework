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
        var settings = new CodeGenerationSettings(basePath, generateMultipleFiles, false, dryRun);

        // Generate code
        var generationTypeNames = new[] { "Entities", "Builders", "BuilderFactory" };
        var generators = typeof(DialogFrameworkCSharpClassBase).Assembly.GetExportedTypes().Where(x => x.BaseType == typeof(DialogFrameworkCSharpClassBase)).ToArray();
        var generationTypes = generators.Where(x => x.Name.EndsWithAny(generationTypeNames));
        var scaffoldingTypes = generators.Where(x => !x.Name.EndsWithAny(generationTypeNames));
        _ = generationTypes.Select(x => (DialogFrameworkCSharpClassBase)Activator.CreateInstance(x)!).Select(x => GenerateCode.For(settings.ForGeneration(), multipleContentBuilder, x)).ToArray();
        _ = scaffoldingTypes.Select(x => (DialogFrameworkCSharpClassBase)Activator.CreateInstance(x)!).Select(x => GenerateCode.For(settings.ForScaffolding(), multipleContentBuilder, x)).ToArray();

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
