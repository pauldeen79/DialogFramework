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
                //.AddMethods(new ClassMethodBuilder()
                //    .WithName("ValidationRule")
                //    .WithOverride()
                //    .AddParameters(new ParameterBuilder().WithName("context").WithType(typeof(object)).WithIsNullable())
                //    .AddParameters(new ParameterBuilder().WithName("secondExpression").WithTypeName("DialogFramework.Domain.Expression"))
                //    .WithTypeName($"{typeof(Result<>).WithoutGenerics()}<{typeof(object).FullName}?>")
                //    .AddLiteralCodeStatements("throw new NotImplementedException();")
                //)
                .Build());

}
