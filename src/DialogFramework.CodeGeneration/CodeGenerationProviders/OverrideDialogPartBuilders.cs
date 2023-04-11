namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideDialogPartBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => $"{Constants.Namespaces.DomainBuilders}/DialogParts";

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override IClass? BaseClass => CreateBaseclass(typeof(IDialogPart), Constants.Namespaces.Domain);
    protected override string BaseClassBuilderNamespace => Constants.Namespaces.DomainBuilders;

    public override object CreateModel()
        => GetImmutableBuilderClasses(
            GetOverrideModels(typeof(IDialogPart)),
            $"{Constants.Namespaces.Domain}.DialogParts",
            $"{Constants.Namespaces.DomainBuilders}.DialogParts");
}
