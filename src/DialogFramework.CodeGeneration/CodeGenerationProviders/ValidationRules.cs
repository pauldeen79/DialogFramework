namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class ValidationRules : DialogFrameworkCSharpClassBase
{
    public override string Path => $"{Constants.Namespaces.Domain}/{nameof(ValidationRules)}";
    public override string LastGeneratedFilesFileName => string.Empty;

    protected override string FileNameSuffix => string.Empty;
    protected override bool CreateCodeGenerationHeader => false;

    public override object CreateModel()
        => GetOverrideModels(typeof(IValidationRule))
            .SelectMany(x => new[]
            {
                new ClassBuilder()
                    .WithNamespace(CurrentNamespace)
                    .WithName(x.Name)
                    .WithPartial()
                    .WithRecord()
                    .AddMethods(new ClassMethodBuilder()
                        .WithName("Validate")
                        .WithOverride()
                        .AddGenericTypeArguments("T")
                        .AddParameters(
                            new ParameterBuilder().WithName("id").WithType(typeof(string)),
                            new ParameterBuilder().WithName("value").WithTypeName("T"),
                            new ParameterBuilder().WithName("dialog").WithTypeName(GetModelTypeName(typeof(IDialog)))
                        )
                        .WithType(typeof(Result))
                        .AddNotImplementedException()
                    )
                    .AddGenericTypeArguments(x.GenericTypeArguments)
                    .AddGenericTypeArgumentConstraints(x.GenericTypeArgumentConstraints)
                    .Build(),
                new ClassBuilder()
                    .WithNamespace(CurrentNamespace)
                    .WithName($"{x.Name}Base")
                    .WithPartial()
                    .WithRecord()
                    .AddMethods(new ClassMethodBuilder()
                        .WithName("Validate")
                        .WithOverride()
                        .AddGenericTypeArguments("T")
                        .AddParameters(
                            new ParameterBuilder().WithName("id").WithType(typeof(string)),
                            new ParameterBuilder().WithName("value").WithTypeName("T"),
                            new ParameterBuilder().WithName("dialog").WithTypeName(GetModelTypeName(typeof(IDialog)))
                        )
                        .WithType(typeof(Result))
                        .AddLiteralCodeStatements($"return {typeof(Result).FullName}.NotSupported();")
                    )
                    .AddGenericTypeArguments(x.GenericTypeArguments)
                    .AddGenericTypeArgumentConstraints(x.GenericTypeArgumentConstraints)
                    .Build(),
            });
}
