namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartBaseBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogParts/Builders";
    public override string DefaultFileName => "Builders.template.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;
    
    protected override bool EnableInheritance => true;

    public override object CreateModel()
        => GetImmutableBuilderClasses(DialogPartBaseModels,
                                      "DialogFramework.Domain.DialogParts",
                                      "DialogFramework.Domain.DialogParts.Builders");
}
