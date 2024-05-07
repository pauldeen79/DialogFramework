namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartResults : DialogFrameworkCSharpClassBase
{
    public DialogPartResults(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
    {
    }

    public override string Path => $"{Constants.Namespaces.Domain}/{nameof(DialogPartResults)}";
    public override string LastGeneratedFilesFilename => string.Empty;

    protected override string FilenameSuffix => string.Empty;
    protected override bool CreateCodeGenerationHeader => false;
    protected override bool SkipWhenFileExists => true;

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => (await GetOverrideModels(typeof(IDialogPartResult)))
            .Select(x => new ClassBuilder()
                .WithNamespace(CurrentNamespace)
                .WithName(x.WithoutInterfacePrefix())
                .WithPartial()
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
