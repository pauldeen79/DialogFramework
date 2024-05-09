namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartResults : DialogFrameworkCSharpClassBase
{
    public DialogPartResults(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    public override string Path => $"{Constants.Namespaces.Domain}/{nameof(DialogPartResults)}";
    public override string LastGeneratedFilesFilename => string.Empty;

    protected override string FilenameSuffix => string.Empty;
    protected override bool CreateCodeGenerationHeader => false;
    protected override bool SkipWhenFileExists => true;
    protected override bool GenerateMultipleFiles => true;

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => (await GetOverrideModels(typeof(IDialogPartResult)))
            .Select(x => new ClassBuilder()
                .WithNamespace(CurrentNamespace)
                .WithName(x.WithoutInterfacePrefix())
                .WithPartial()
                .WithRecord()
                .AddMethods(new MethodBuilder()
                    .WithName("GetValue")
                    .WithOverride()
                    .WithReturnTypeName($"{typeof(Result<>).WithoutGenerics()}<{typeof(object).FullName}?>")
                    .NotImplemented()
                )
                .AddGenericTypeArguments(x.GenericTypeArguments)
                .AddGenericTypeArgumentConstraints(x.GenericTypeArgumentConstraints)
                .Build());
}
