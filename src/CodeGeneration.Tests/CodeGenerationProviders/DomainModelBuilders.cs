namespace CodeGeneration.Tests.CodeGenerationProviders;

public class DomainModelBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Core/DomainModel/Builders";

    public override string DefaultFileName => "Builders.generated.cs";

    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableBuilderClasses(GetDomainModelModels(),
                                      "DialogFramework.Core.DomainModel",
                                      "DialogFramework.Core.DomainModel.Builders");
}
