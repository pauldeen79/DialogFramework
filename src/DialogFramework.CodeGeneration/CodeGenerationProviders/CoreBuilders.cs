namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain.Builders";
    public override string DefaultFileName => "Builders.template.generated.cs";

    public override object CreateModel()
        => GetImmutableBuilderClasses(
            GetCoreModels(),
            "DialogFramework.Domain",
            "DialogFramework.Domain.Builders");
}
