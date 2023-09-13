namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartModelFactory : DialogFrameworkModelClassBase
{
    public override string Path => Constants.Namespaces.DomainModels;

    public override object CreateModel()
        => CreateBuilderFactoryModels(
            GetOverrideModels(typeof(IDialogPart)),
            new(Constants.Namespaces.DomainModels,
            nameof(DialogPartModelFactory),
            $"{Constants.Namespaces.Domain}.DialogPart",
            $"{Constants.Namespaces.DomainModels}.DialogParts",
            "DialogPartModel",
            $"{Constants.Namespaces.Domain}.DialogParts"));
}
