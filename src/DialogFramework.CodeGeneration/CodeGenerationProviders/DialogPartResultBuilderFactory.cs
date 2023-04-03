namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartResultBuilderFactory : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain.Builders";
    public override string DefaultFileName => "Builders.template.generated.cs";

    public override object CreateModel()
        => CreateBuilderFactoryModels(
            GetOverrideModels(typeof(IDialogPartResult)),
            new("DialogFramework.Domain.Builders",
            "DialogPartResultBuilderFactory",
            "DialogFramework.Domain.DialogPartResult",
            "DialogFramework.Domain.Builders.DialogPartResults",
            "DialogPartResultBuilder",
            "DialogFramework.Domain.DialogPartResults"));
}
