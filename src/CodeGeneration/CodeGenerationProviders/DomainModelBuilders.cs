namespace CodeGeneration.CodeGenerationProviders;

public class DomainModelBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Core/DomainModel/Builders";

    public override string DefaultFileName => "Builders.template.generated.cs";

    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableBuilderClasses(GetDomainModelModels(),
                                      "DialogFramework.Core.DomainModel",
                                      "DialogFramework.Core.DomainModel.Builders");
}
