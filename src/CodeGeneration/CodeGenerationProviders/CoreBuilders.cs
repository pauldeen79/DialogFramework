namespace CodeGeneration.CodeGenerationProviders;

public class CoreBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Core/Builders";

    public override string DefaultFileName => "Builders.template.generated.cs";

    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableBuilderClasses(GetCoreModels(),
                                      "DialogFramework.Core",
                                      "DialogFramework.Core.Builders");
}
