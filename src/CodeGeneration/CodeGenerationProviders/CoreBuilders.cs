namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/Builders";
    public override string DefaultFileName => "Builders.template.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableBuilderClasses(CoreModels.Where(x => !x.Name.EndsWith("Arguments")).ToArray(),
                                      "DialogFramework.Domain",
                                      "DialogFramework.Domain.Builders");
}
