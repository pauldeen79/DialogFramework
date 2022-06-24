namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreRecords : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain";
    public override string DefaultFileName => "Entities.template.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableClasses(CoreModels, "DialogFramework.Domain");
}
