namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideDialogPartResultBuilders : DialogFrameworkCSharpClassBase
{
    public OverrideDialogPartResultBuilders(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
    {
    }

    public override string Path => $"{Constants.Paths.DomainBuilders}/DialogPartResults";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override async Task<TypeBase?> GetBaseClass()  => await CreateBaseClass(typeof(IDialogPartResult), Constants.Namespaces.Domain);
    protected override string BaseClassBuilderNamespace => Constants.Namespaces.DomainBuilders;
    protected override ArgumentValidationType ValidateArgumentsInConstructor => ArgumentValidationType.None; // there are no properties in DialogPartResults, so this is not necessary

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => await GetBuilders(
            await GetOverrideModels(typeof(IDialogPartResult)),
            $"{Constants.Namespaces.DomainBuilders}.DialogPartResults",
            $"{Constants.Namespaces.Domain}.DialogPartResults");
}
