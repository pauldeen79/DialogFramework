namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreEntities : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain";
    public override string DefaultFileName => "Entities.template.generated.cs";

    public override object CreateModel()
        => GetImmutableClasses(GetCoreModels(), "DialogFramework.Domain");
}
