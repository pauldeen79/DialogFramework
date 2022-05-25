namespace CodeGeneration.Tests.CodeGenerationProviders;

public class CoreModels : CSharpExpressionDumperClassBase
{
    public override string Path => "CodeGeneration.Tests/CodeGenerationProviders";
    public override string DefaultFileName => "DialogFrameworkCSharpClassBase.Core.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;

    protected override string Namespace => "CodeGeneration.Tests.CodeGenerationProviders";
    protected override string ClassName => "DialogFrameworkCSharpClassBase";
    protected override string MethodName => "GetCoreModels";

    protected override string[] NamespacesToAbbreviate => new[]
    {
        "System.Collections.Generic",
        "ModelFramework.Objects.Builders",
        "ModelFramework.Objects.Contracts"
    };

    protected override Type[] Models => new[]
    {
        typeof(IDialogContext),
    };
}
