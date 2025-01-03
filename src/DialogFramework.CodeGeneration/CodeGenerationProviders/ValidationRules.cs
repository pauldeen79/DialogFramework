﻿namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class ValidationRules(IPipelineService pipelineService) : DialogFrameworkCSharpClassBase(pipelineService)
{
    public override string Path => $"{Constants.Namespaces.Domain}/{nameof(ValidationRules)}";
    public override string LastGeneratedFilesFilename => string.Empty;

    protected override string FilenameSuffix => string.Empty;
    protected override bool CreateCodeGenerationHeader => false;
    protected override bool SkipWhenFileExists => true;
    protected override bool GenerateMultipleFiles => true;

    public override async Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken)
        => (await GetOverrideModels(typeof(IValidationRule)))
            .OnSuccess(result =>
                Result.Success(result.Value!.Select(x => new ClassBuilder()
                .WithNamespace(CurrentNamespace)
                .WithName(x.WithoutInterfacePrefix())
                .WithPartial()
                .WithRecord()
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
            )));
}
