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
                .WithNamespace("DialogFramework.Domain.ValidationRules")
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
                        new ParameterBuilder().WithName("dialog").WithTypeName(nameof(IDialog).Substring(1)) //TODO: Make a little easier to use. We want to transform CodeGeneration model into the domain model type, like .WithModelTypeName(typeof(IDialog), GetCoreModels())
                    )
                    .WithType(typeof(Result))
                    .AddLiteralCodeStatements("throw new NotImplementedException();") //TODO: Add method in ModelFramework to make this better, like .AddNotImplementedException()
                )
                .Build());
}
