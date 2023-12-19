namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartBuilderFactory : DialogFrameworkCSharpClassBase
{
    public override string Path => Constants.Namespaces.DomainBuilders;

    public override object CreateModel()
        => CreateBuilderFactories(
            GetOverrideModels(typeof(IDialogPart)),
            new(Constants.Namespaces.DomainBuilders,
            nameof(DialogPartBuilderFactory),
            $"{Constants.Namespaces.Domain}.DialogPart",
            $"{Constants.Namespaces.DomainBuilders}.DialogParts",
            "DialogPartBuilder",
            $"{Constants.Namespaces.Domain}.DialogParts"));
}
