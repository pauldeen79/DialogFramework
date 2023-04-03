namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideDialogPartEntities : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogParts";
    public override string DefaultFileName => "Entities.generated.cs";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override IClass? BaseClass => CreateBaseclass(typeof(IDialogPart), "DialogFramework.Domain");
    protected override bool ValidateArgumentsInConstructor => false; // there are no properties in DialogParts, so this is not necessary

    public override object CreateModel()
        => GetImmutableClasses(GetOverrideModels(typeof(IDialogPart)), "DialogFramework.Domain.DialogParts");
}
