namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideValidationRuleEntities : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/ValidationRules";
    public override string DefaultFileName => "Entities.generated.cs";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override IClass? BaseClass => CreateBaseclass(typeof(IValidationRule), "DialogFramework.Domain");
    protected override bool ValidateArgumentsInConstructor => false; // there are no properties in ValidationRules, so this is not necessary

    public override object CreateModel()
        => GetImmutableClasses(GetOverrideModels(typeof(IValidationRule)), "DialogFramework.Domain.ValidationRules");
}
