namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartBaseRecords : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogParts";
    public override string DefaultFileName => "Entities.template.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;

    protected override bool EnableInheritance => true;

    public override object CreateModel()
        => GetImmutableClasses(DialogPartBaseModels, "DialogFramework.Domain.DialogParts");
}
