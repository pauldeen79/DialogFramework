namespace CodeGeneration.Tests.CodeGenerationProviders;

public class CoreBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Core/Builders";

    public override string DefaultFileName => "Builders.generated.cs";

    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableBuilderClasses(GetCoreModelTypes(),
                                      "DialogFramework.Core",
                                      "DialogFramework.Core.Builders");
}
