namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartResultModelFactory : DialogFrameworkModelClassBase
{
    public override string Path => Constants.Namespaces.DomainModels;

    public override object CreateModel()
        => CreateBuilderFactoryModels(
            GetOverrideModels(typeof(IDialogPartResult)),
            new(Constants.Namespaces.DomainModels,
            nameof(DialogPartResultModelFactory),
            $"{Constants.Namespaces.Domain}.DialogPartResult",
            $"{Constants.Namespaces.DomainModels}.DialogPartResults",
            "DialogPartResultModel",
            $"{Constants.Namespaces.Domain}.DialogPartResults"));
}
