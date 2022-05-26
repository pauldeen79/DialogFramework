namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DomainModelRecords : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Core/DomainModel";

    public override string DefaultFileName => "Entities.template.generated.cs";

    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableClasses(GetDomainModelModels(), "DialogFramework.Core.DomainModel");
}
