namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class ValidationRuleBuilderFactory : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain.Builders";
    public override string DefaultFileName => "Builders.template.generated.cs";

    public override object CreateModel()
        => CreateBuilderFactoryModels(
            GetOverrideModels(typeof(IValidationRule)),
            new("DialogFramework.Domain.Builders",
            "ValidationRuleBuilderFactory",
            "DialogFramework.Domain.ValidationRule",
            "DialogFramework.Domain.Builders.ValidationRules",
            "ValidationRuleBuilder",
            "DialogFramework.Domain.ValidationRules"));
}
