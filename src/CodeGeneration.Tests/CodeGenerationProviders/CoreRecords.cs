namespace CodeGeneration.Tests.CodeGenerationProviders;

public class CoreRecords : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.UniversalModel";

    public override string DefaultFileName => "Entities.generated.cs";

    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableClasses(GetCoreModelTypes(), "DialogFramework.UniversalModel");
}
