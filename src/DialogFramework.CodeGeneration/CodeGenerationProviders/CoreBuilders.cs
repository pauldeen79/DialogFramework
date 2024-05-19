namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreBuilders : DialogFrameworkCSharpClassBase
{
    public CoreBuilders(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override string Path => Constants.Paths.DomainBuilders;

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetBuilders(await GetCoreModels(), CurrentNamespace, Constants.Namespaces.Domain);
}
