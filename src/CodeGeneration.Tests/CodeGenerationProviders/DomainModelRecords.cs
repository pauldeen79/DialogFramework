namespace CodeGeneration.Tests.CodeGenerationProviders;

public class DomainModelRecords : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.UniversalModel/DomainModel";

    public override string DefaultFileName => "Entities.generated.cs";

    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableClasses(GetDomainModelModelTypes(), "DialogFramework.UniversalModel.DomainModel");
}
