namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => Constants.Namespaces.DomainBuilders;

    public override object CreateModel()
        => GetImmutableBuilderClasses(
            GetCoreModels(),
            Constants.Namespaces.Domain,
            Constants.Namespaces.DomainBuilders);
}
