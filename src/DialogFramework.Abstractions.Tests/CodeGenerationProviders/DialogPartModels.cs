namespace DialogFramework.Abstractions.Tests.CodeGenerationProviders;

public class DialogPartModels : CSharpExpressionDumperClassBase
{
    public override string Path => "CodeGeneration/CodeGenerationProviders";
    public override string DefaultFileName => "DialogFrameworkCSharpClassBase.DialogPart.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;

    protected override string Namespace => "CodeGeneration.CodeGenerationProviders";
    protected override string ClassName => "DialogFrameworkCSharpClassBase";
    protected override string MethodName => "GetDialogPartModels";

    protected override string[] NamespacesToAbbreviate => new[]
    {
        "System.Collections.Generic",
        "ModelFramework.Objects.Builders",
        "ModelFramework.Objects.Contracts"
    };

    protected override Type[] Models => new[]
    {
        typeof(IAbortedDialogPart),
        typeof(ICompletedDialogPart),
        typeof(IDecisionDialogPart),
        typeof(IErrorDialogPart),
        typeof(IMessageDialogPart),
        typeof(INavigationDialogPart),
        typeof(IQuestionDialogPart),
        typeof(IRedirectDialogPart),
    };
}
