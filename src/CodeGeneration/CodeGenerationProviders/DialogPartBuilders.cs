namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogParts/Builders";
    public override string DefaultFileName => "Builders.template.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;
    
    protected override bool EnableInheritance => true;
    protected override IClass? BaseClass => GetDialogPartBaseClass();

    public override object CreateModel()
        => GetImmutableBuilderClasses(DialogPartModels,
                                      "DialogFramework.Domain.DialogParts",
                                      "DialogFramework.Domain.DialogParts.Builders");
}
