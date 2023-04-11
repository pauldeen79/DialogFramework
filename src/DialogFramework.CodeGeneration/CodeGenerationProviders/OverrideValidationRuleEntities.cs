namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideValidationRuleEntities : DialogFrameworkCSharpClassBase
{
    public override string Path => $"{Constants.Namespaces.Domain}/ValidationRules";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override IClass? BaseClass => CreateBaseclass(typeof(IValidationRule), Constants.Namespaces.Domain);

    public override object CreateModel()
        => GetImmutableClasses(GetOverrideModels(typeof(IValidationRule)), $"{Constants.Namespaces.Domain}.ValidationRules");
}
