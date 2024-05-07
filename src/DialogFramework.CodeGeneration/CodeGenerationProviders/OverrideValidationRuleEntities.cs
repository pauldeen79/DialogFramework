namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideValidationRuleEntities : DialogFrameworkCSharpClassBase
{
    public OverrideValidationRuleEntities(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
    {
    }

    public override string Path => $"{Constants.Paths.Domain}/ValidationRules";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    //protected override bool AddNullChecks => false; // seems to be necessary :(
    protected override async Task<TypeBase?> GetBaseClass() => await CreateBaseClass(typeof(IValidationRule), Constants.Namespaces.Domain);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetOverrideModels(typeof(IValidationRule)), $"{Constants.Namespaces.Domain}.ValidationRules");
}
