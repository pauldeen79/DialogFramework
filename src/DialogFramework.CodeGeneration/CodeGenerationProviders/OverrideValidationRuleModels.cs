namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideValidationRuleModels : DialogFrameworkModelClassBase
{
    public override string Path => $"{Constants.Namespaces.DomainModels}/ValidationRules";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override IClass? BaseClass => CreateBaseclass(typeof(IValidationRule), Constants.Namespaces.Domain);
    protected override string BaseClassBuilderNamespace => Constants.Namespaces.DomainModels;

    public override object CreateModel()
        => GetImmutableBuilderClasses(
            GetOverrideModels(typeof(IValidationRule)),
            $"{Constants.Namespaces.Domain}.ValidationRules",
            $"{Constants.Namespaces.DomainModels}.ValidationRules");
}
