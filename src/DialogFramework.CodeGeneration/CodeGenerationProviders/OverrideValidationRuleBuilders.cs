namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideValidationRuleBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain.Builders/ValidationRules";
    public override string DefaultFileName => "Builders.generated.cs";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override IClass? BaseClass => CreateBaseclass(typeof(IValidationRule), "DialogFramework.Domain");
    protected override string BaseClassBuilderNamespace => "DialogFramework.Domain.Builders";

    public override object CreateModel()
        => GetImmutableBuilderClasses(
            GetOverrideModels(typeof(IValidationRule)),
            "DialogFramework.Domain.ValidationRules",
            "DialogFramework.Domain.Builders.ValidationRules");
}
