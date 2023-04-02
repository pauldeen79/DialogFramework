namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideDialogPartBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain.Builders/DialogParts";
    public override string DefaultFileName => "Builders.generated.cs";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override IClass? BaseClass => CreateBaseclass(typeof(IDialogPart), "DialogFramework.Domain");
    protected override string BaseClassBuilderNamespace => "DialogFramework.Domain.Builders";

    public override object CreateModel()
        => GetImmutableBuilderClasses(
            GetOverrideModels(typeof(IDialogPart)),
            "DialogFramework.Domain.DialogParts",
            "DialogFramework.Domain.Builders.DialogParts");
}
