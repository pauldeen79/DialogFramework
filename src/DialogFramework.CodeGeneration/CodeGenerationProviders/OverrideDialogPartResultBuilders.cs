namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideDialogPartResultBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain.Builders/DialogPartResults";
    public override string DefaultFileName => "Builders.generated.cs";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override IClass? BaseClass => CreateBaseclass(typeof(IDialogPartResult), "DialogFramework.Domain");
    protected override string BaseClassBuilderNamespace => "DialogFramework.Domain.Builders";

    public override object CreateModel()
        => GetImmutableBuilderClasses(
            GetOverrideModels(typeof(IDialogPartResult)),
            "DialogFramework.Domain.DialogPartResults",
            "DialogFramework.Domain.Builders.DialogPartResults");
}
