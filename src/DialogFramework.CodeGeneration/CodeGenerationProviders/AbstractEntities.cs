namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractEntities : DialogFrameworkCSharpClassBase
{
    public AbstractEntities(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override string Path => Constants.Paths.Domain;

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override bool IsAbstract => true;
    //protected override bool AddNullChecks => false; // seems to be necessary :(

    protected override ArgumentValidationType ValidateArgumentsInConstructor => ArgumentValidationType.None; // not needed for abstract entities, because each derived class will do its own validation

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetAbstractModels(), CurrentNamespace);
}
