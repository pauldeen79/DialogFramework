namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreEntities : DialogFrameworkCSharpClassBase
{
    public CoreEntities(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
    {
    }

    //protected override bool AddNullChecks => false; // seems to be necessary :(

    public override string Path => Constants.Paths.Domain;

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetCoreModels(), Constants.Namespaces.Domain);
}
