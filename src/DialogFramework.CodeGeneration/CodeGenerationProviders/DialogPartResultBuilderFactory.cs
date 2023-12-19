namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartResultBuilderFactory : DialogFrameworkCSharpClassBase
{
    public override string Path => Constants.Namespaces.DomainBuilders;

    public override object CreateModel()
        => CreateBuilderFactories(
            GetOverrideModels(typeof(IDialogPartResult)),
            new(Constants.Namespaces.DomainBuilders,
            nameof(DialogPartResultBuilderFactory),
            $"{Constants.Namespaces.Domain}.DialogPartResult",
            $"{Constants.Namespaces.DomainBuilders}.DialogPartResults",
            "DialogPartResultBuilder",
            $"{Constants.Namespaces.Domain}.DialogPartResults"));
}
