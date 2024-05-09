namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractNonGenericBuilders : DialogFrameworkCSharpClassBase
{
    public AbstractNonGenericBuilders(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override string Path => Constants.Paths.DomainBuilders;

    protected override bool AddNullChecks => false; // not needed for abstract builders, because each derived class will do its own validation
    protected override bool AddBackingFields => true; // backing fields are added when using null checks... so we need to add this explicitly

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override bool IsAbstract => true;
    protected override bool CreateAsObservable => true;

    protected override string FilenameSuffix => ".nongeneric.template.generated";

    // Do not generate 'With' methods. This does not work out of the box, for some reason :(
    protected override string SetMethodNameFormatString => string.Empty;
    protected override string AddMethodNameFormatString => string.Empty;

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetNonGenericBuilders(await GetAbstractModels(), CurrentNamespace, Constants.Namespaces.Domain);
}
