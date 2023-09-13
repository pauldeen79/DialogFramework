namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideDialogPartModels : DialogFrameworkModelClassBase
{
    public override string Path => $"{Constants.Namespaces.DomainModels}/DialogParts";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override IClass? BaseClass => CreateBaseclass(typeof(IDialogPart), Constants.Namespaces.Domain);
    protected override string BaseClassBuilderNamespace => Constants.Namespaces.DomainModels;

    public override object CreateModel()
        => GetImmutableBuilderClasses(
            GetOverrideModels(typeof(IDialogPart)),
            $"{Constants.Namespaces.Domain}.DialogParts",
            $"{Constants.Namespaces.DomainModels}.DialogParts");
}
