namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartBuilderFactory : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain.Builders";
    public override string DefaultFileName => "Builders.template.generated.cs";

    public override object CreateModel()
        => CreateBuilderFactoryModels(
            GetOverrideModels(typeof(IDialogPart)),
            new("DialogFramework.Domain.Builders",
            "DialogPartBuilderFactory",
            "DialogFramework.Domain.DialogPart",
            "DialogFramework.Domain.Builders.DialogParts",
            "DialogPartBuilder",
            "DialogFramework.Domain.DialogParts"));
}
