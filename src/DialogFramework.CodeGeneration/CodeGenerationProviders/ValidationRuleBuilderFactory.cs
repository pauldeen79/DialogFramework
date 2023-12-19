namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class ValidationRuleBuilderFactory : DialogFrameworkCSharpClassBase
{
    public override string Path => Constants.Namespaces.DomainBuilders;

    public override object CreateModel()
        => CreateBuilderFactories(
            GetOverrideModels(typeof(IValidationRule)),
            new(Constants.Namespaces.DomainBuilders,
            nameof(ValidationRuleBuilderFactory),
            $"{Constants.Namespaces.Domain}.ValidationRule",
            $"{Constants.Namespaces.DomainBuilders}.ValidationRules",
            "ValidationRuleBuilder",
            $"{Constants.Namespaces.Domain}.ValidationRules"));
}
