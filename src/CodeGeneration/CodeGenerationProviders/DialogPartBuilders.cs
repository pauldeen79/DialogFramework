namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DomainModel/DialogParts/Builders";

    public override string DefaultFileName => "Builders.template.generated.cs";

    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableBuilderClasses(GetDialogPartModels(),
                                      "DialogFramework.Domain.DomainModel.DialogParts",
                                      "DialogFramework.Domain.DomainModel.DialogParts.Builders");
}
