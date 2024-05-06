namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideDialogPartEntities : DialogFrameworkCSharpClassBase
{
    public OverrideDialogPartEntities(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
    {
    }

    public override string Path => $"{Constants.Paths.Domain}/DialogParts";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    //protected override bool AddNullChecks => false; // seems to be necessary :(
    protected override async Task<TypeBase?> GetBaseClass() => await CreateBaseClass(typeof(IDialogPart), Constants.Namespaces.Domain);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetOverrideModels(typeof(IDialogPart)), $"{Constants.Namespaces.Domain}.DialogParts");
}
