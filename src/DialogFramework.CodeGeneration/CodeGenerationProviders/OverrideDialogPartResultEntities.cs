namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideDialogPartResultEntities : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogPartResults";
    public override string DefaultFileName => "Entities.generated.cs";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override IClass? BaseClass => CreateBaseclass(typeof(IDialogPartResult), "DialogFramework.Domain");
    protected override bool ValidateArgumentsInConstructor => false; // there are no properties in DialogPartResults, so this is not necessary

    public override object CreateModel()
        => GetImmutableClasses(GetOverrideModels(typeof(IDialogPartResult)), "DialogFramework.Domain.DialogPartResults");
}
