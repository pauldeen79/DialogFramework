namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DomainModelBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DomainModel/Builders";

    public override string DefaultFileName => "Builders.template.generated.cs";

    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableBuilderClasses(GetDomainModelModels(),
                                      "DialogFramework.Domain.DomainModel",
                                      "DialogFramework.Domain.DomainModel.Builders");
}
