namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractBuilders : DialogFrameworkCSharpClassBase
{
    public AbstractBuilders(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override string Path => Constants.Paths.DomainBuilders;

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override bool IsAbstract => true;

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetBuilders(await GetAbstractModels(), CurrentNamespace, Constants.Namespaces.Domain);
}
