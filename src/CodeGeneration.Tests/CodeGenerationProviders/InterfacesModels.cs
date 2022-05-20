using DialogFramework.Abstractions.DomainModel;
using DialogFramework.Abstractions.DomainModel.DialogParts;

namespace CodeGeneration.Tests.CodeGenerationProviders;

public class InterfacesModels : CSharpExpressionDumperClassBase
{
    public override string Path => "CodeGeneration.Tests/CodeGenerationProviders";
    public override string DefaultFileName => "DialogFrameworkCSharpClassBase.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;

    protected override string Namespace => "CodeGeneration.Tests.CodeGenerationProviders";
    protected override string ClassName => "DialogFrameworkCSharpClassBase";

    protected override string[] NamespacesToAbbreviate => new[]
    {
        "System.Collections.Generic",
        "ModelFramework.Objects.Builders",
        "ModelFramework.Objects.Contracts"
    };

    protected override Type[] Models => new[]
    {
        typeof(IDialog),
        typeof(IDialogMetadata),
        typeof(IDialogPartGroup),
        typeof(IDialogPartResult),
        typeof(IDialogPartResultDefinition),
        typeof(IDialogPartResultValue),
        typeof(IDialogValidationResult),

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
