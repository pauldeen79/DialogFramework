namespace CodeGeneration.Tests.CodeGenerationProviders;

public class DomainModelModels : CSharpExpressionDumperClassBase
{
    public override string Path => "CodeGeneration.Tests/CodeGenerationProviders";
    public override string DefaultFileName => "DialogFrameworkCSharpClassBase.DomainModel.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;

    protected override string Namespace => "CodeGeneration.Tests.CodeGenerationProviders";
    protected override string ClassName => "DialogFrameworkCSharpClassBase";
    protected override string MethodName => "GetDomainModelModels";

    protected override string[] NamespacesToAbbreviate => new[]
    {
        "System.Collections.Generic",
        "ModelFramework.Objects.Builders",
        "ModelFramework.Objects.Contracts"
    };

    protected override Type[] Models => new[]
    {
        typeof(IDecision),
        typeof(IDialog),
        typeof(IDialogIdentifier),
        typeof(IDialogMetadata),
        typeof(IDialogPartGroup),
        typeof(IDialogPartResult),
        typeof(IDialogPartResultDefinition),
        typeof(IDialogPartResultValue),
        typeof(IDialogValidationResult),
    };
}
