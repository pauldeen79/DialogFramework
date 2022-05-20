namespace CodeGeneration.Tests.CodeGenerationProviders;

public class CoreBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.UniversalModel/Builders";

    public override string DefaultFileName => "Builders.generated.cs";

    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableBuilderClasses(GetCoreModelTypes(),
                                      "DialogFramework.UniversalModel",
                                      "DialogFramework.UniversalModel.Builders");
}
