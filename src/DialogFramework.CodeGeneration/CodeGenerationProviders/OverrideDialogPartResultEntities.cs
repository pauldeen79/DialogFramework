namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideDialogPartResultEntities : DialogFrameworkCSharpClassBase
{
    public OverrideDialogPartResultEntities(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
    {
    }

    public override string Path => $"{Constants.Paths.Domain}/DialogPartResults";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    //protected override bool AddNullChecks => false; // seems to be necessary :(
    protected override async Task<TypeBase?> GetBaseClass() => await CreateBaseClass(typeof(IDialogPartResult), Constants.Namespaces.Domain);
    protected override ArgumentValidationType ValidateArgumentsInConstructor => ArgumentValidationType.None; // there are no properties in DialogPartResults, so this is not necessary

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetOverrideModels(typeof(IDialogPartResult)), $"{Constants.Namespaces.Domain}.DialogPartResults");
}
