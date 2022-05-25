namespace CodeGeneration.Tests.CodeGenerationProviders;

public class DialogPartRecords : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Core/DomainModel/DialogParts";

    public override string DefaultFileName => "Entities.generated.cs";

    public override bool RecurseOnDeleteGeneratedFiles => false;

    public override object CreateModel()
        => GetImmutableClasses(GetDialogPartModels(), "DialogFramework.Core.DomainModel.DialogParts");
}
