namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartRecords : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogParts";
    public override string DefaultFileName => "Entities.template.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;

    protected override bool EnableInheritance => true;
    protected override IClass? BaseClass => GetDialogPartBaseClass();

    public override object CreateModel()
        => GetImmutableClasses(DialogPartModels, "DialogFramework.Domain.DialogParts");
}
