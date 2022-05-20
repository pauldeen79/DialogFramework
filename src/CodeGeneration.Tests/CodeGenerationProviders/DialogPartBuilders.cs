namespace CodeGeneration.Tests.CodeGenerationProviders;

public class DialogPartBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.UniversalModel/DomainModel/DialogParts/Builders";

    public override string DefaultFileName => "Builders.generated.cs";

    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableBuilderClasses(GetDialogPartModelTypes(),
                                      "DialogFramework.UniversalModel.DomainModel.DialogParts",
                                      "DialogFramework.UniversalModel.DomainModel.DialogParts.Builders");
}
