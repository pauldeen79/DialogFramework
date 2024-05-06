namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideValidationRuleBuilders : DialogFrameworkCSharpClassBase
{
    public OverrideValidationRuleBuilders(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
    {
    }

    public override string Path => $"{Constants.Paths.DomainBuilders}/ValidationRules";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override async Task<TypeBase?> GetBaseClass() => await CreateBaseClass(typeof(IValidationRule), Constants.Namespaces.Domain);
    protected override string BaseClassBuilderNamespace => Constants.Namespaces.DomainBuilders;

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetBuilders(
            await GetOverrideModels(typeof(IValidationRule)),
            $"{Constants.Namespaces.DomainBuilders}.ValidationRules",
            $"{Constants.Namespaces.Domain}.ValidationRules");
}
