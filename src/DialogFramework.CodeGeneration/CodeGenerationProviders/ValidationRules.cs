namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class ValidationRules : DialogFrameworkCSharpClassBase
{
    public ValidationRules(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
    {
    }

    public override string Path => $"{Constants.Namespaces.Domain}/{nameof(ValidationRules)}";
    public override string LastGeneratedFilesFilename => string.Empty;

    protected override string FilenameSuffix => string.Empty;
    protected override bool CreateCodeGenerationHeader => false;
    protected override bool SkipWhenFileExists => true;

    public override async Task<IEnumerable<TypeBase>> GetModel()
        => (await GetOverrideModels(typeof(IValidationRule)))
            .SelectMany(x => new[]
            {
                new ClassBuilder()
                    .WithNamespace(CurrentNamespace)
                    .WithName(x.WithoutInterfacePrefix())
                    .WithPartial()
                    .AddMethods(new MethodBuilder()
                        .WithName("Validate")
                        .WithOverride()
                        .AddGenericTypeArguments("T")
                        .AddParameters(
                            new ParameterBuilder().WithName("id").WithType(typeof(string)),
                            new ParameterBuilder().WithName("value").WithTypeName("T"),
                            new ParameterBuilder().WithName("dialog").WithType(typeof(IDialog))
                        )
                        .WithReturnType(typeof(Result))
                        .NotImplemented()
                    )
                    .AddGenericTypeArguments(x.GenericTypeArguments)
                    .AddGenericTypeArgumentConstraints(x.GenericTypeArgumentConstraints)
                    .Build()
            });
}
