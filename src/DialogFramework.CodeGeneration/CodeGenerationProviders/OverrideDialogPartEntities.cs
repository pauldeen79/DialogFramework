namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideDialogPartEntities : DialogFrameworkCSharpClassBase
{
    public OverrideDialogPartEntities(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override string Path => $"{Constants.Paths.Domain}/DialogParts";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override async Task<TypeBase?> GetBaseClass() => await CreateBaseClass(typeof(IDialogPart), Constants.Namespaces.Domain);

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetEntities(await GetOverrideModels(typeof(IDialogPart)), CurrentNamespace);
}
