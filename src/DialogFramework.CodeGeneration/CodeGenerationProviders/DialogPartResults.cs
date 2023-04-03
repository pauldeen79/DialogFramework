namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartResults : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogPartResults";
    public override string DefaultFileName => "DialogPartResults.cs";
    public override string LastGeneratedFilesFileName => string.Empty;

    protected override string FileNameSuffix => string.Empty;
    protected override bool CreateCodeGenerationHeader => false;

    public override object CreateModel()
        => GetOverrideModels(typeof(IDialogPartResult))
            .Select(x => new ClassBuilder()
                .WithNamespace("DialogFramework.Domain.DialogPartResults")
                .WithName(x.Name)
                .WithPartial()
                .WithRecord()
                //.AddMethods(new ClassMethodBuilder()
                //    .WithName("DialogPartResult")
                //    .WithOverride()
                //    .AddParameters(new ParameterBuilder().WithName("context").WithType(typeof(object)).WithIsNullable())
                //    .AddParameters(new ParameterBuilder().WithName("secondExpression").WithTypeName("DialogFramework.Domain.Expression"))
                //    .WithTypeName($"{typeof(Result<>).WithoutGenerics()}<{typeof(object).FullName}?>")
                //    .AddLiteralCodeStatements("throw new NotImplementedException();")
                //)
                .Build());

}
