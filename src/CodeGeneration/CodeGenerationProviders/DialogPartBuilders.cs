namespace CodeGeneration.CodeGenerationProviders;

public class DialogPartBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Core/DomainModel/DialogParts/Builders";

    public override string DefaultFileName => "Builders.template.generated.cs";

    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableBuilderClasses(GetDialogPartModels(),
                                      "DialogFramework.Core.DomainModel.DialogParts",
                                      "DialogFramework.Core.DomainModel.DialogParts.Builders");
}
