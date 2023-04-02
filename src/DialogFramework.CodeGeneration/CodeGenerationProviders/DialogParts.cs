namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogParts : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogParts";
    public override string DefaultFileName => "DialogParts.cs";
    public override string LastGeneratedFilesFileName => string.Empty;

    protected override string FileNameSuffix => string.Empty;
    protected override bool CreateCodeGenerationHeader => false;

    public override object CreateModel()
        => GetOverrideModels(typeof(IDialogPart))
            .Select(x => new ClassBuilder()
                .WithNamespace("DialogFramework.Domain.DialogParts")
                .WithName(x.Name)
                .WithPartial()
                .WithRecord()
                //.AddMethods(new ClassMethodBuilder()
                //    .WithName("DialogPart")
                //    .WithOverride()
                //    .AddParameters(new ParameterBuilder().WithName("context").WithType(typeof(object)).WithIsNullable())
                //    .AddParameters(new ParameterBuilder().WithName("secondExpression").WithTypeName("DialogFramework.Domain.Expression"))
                //    .WithTypeName($"{typeof(Result<>).WithoutGenerics()}<{typeof(object).FullName}?>")
                //    .AddLiteralCodeStatements("throw new NotImplementedException();")
                //)
                .Build());

}
