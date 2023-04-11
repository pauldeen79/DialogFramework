namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreEntities : DialogFrameworkCSharpClassBase
{
    public override string Path => Constants.Namespaces.Domain;

    public override object CreateModel()
        => GetImmutableClasses(GetCoreModels(), Constants.Namespaces.Domain);
}
