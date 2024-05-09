namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreEntities : DialogFrameworkCSharpClassBase
{
    public CoreEntities(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override string Path => Constants.Paths.Domain;

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetCoreModels(), CurrentNamespace);
}
