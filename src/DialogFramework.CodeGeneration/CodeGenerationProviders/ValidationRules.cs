namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class ValidationRules : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/ValidationRules";
    public override string DefaultFileName => "ValidationRules.cs";
    public override string LastGeneratedFilesFileName => string.Empty;

    protected override string FileNameSuffix => string.Empty;
    protected override bool CreateCodeGenerationHeader => false;

    public override object CreateModel()
        => GetOverrideModels(typeof(IValidationRule))
            .Select(x => new ClassBuilder()
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
                .Build());
}
