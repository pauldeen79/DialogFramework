namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class ValidationRuleModelFactory : DialogFrameworkModelClassBase
{
    public override string Path => Constants.Namespaces.DomainModels;

    public override object CreateModel()
        => CreateBuilderFactories(
            GetOverrideModels(typeof(IValidationRule)),
            new(Constants.Namespaces.DomainModels,
            nameof(ValidationRuleModelFactory),
            $"{Constants.Namespaces.Domain}.ValidationRule",
            $"{Constants.Namespaces.DomainModels}.ValidationRules",
            "ValidationRuleModel",
            $"{Constants.Namespaces.Domain}.ValidationRules"));
}
