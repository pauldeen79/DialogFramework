namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public abstract class DialogFrameworkModelClassBase : DialogFrameworkCSharpClassBase
{
    protected override string AddMethodNameFormatString => string.Empty; // we don't want Add methods for collection properties
    protected override string SetMethodNameFormatString => string.Empty; // we don't want With methods for non-collection properties
    protected override string BuilderNameFormatString => "{0}Model";
    protected override string BuilderBuildMethodName => "ToEntity";
    protected override string BuilderFactoryName => "ModelFactory";
    protected override string BuilderBuildTypedMethodName => "ToTypedEntity";
    protected override string BuilderName => "Model";
    protected override string BuildersName => "Models";
    protected override bool UseLazyInitialization => false; // we don't want lazy stuff in models, just getters and setters
    protected override bool ConvertStringToStringBuilderOnBuilders => false; // we don't want string builders in models, just strings

    protected override IEnumerable<KeyValuePair<string, string>> GetCustomBuilderNamespaceMapping()
    {
        yield return new KeyValuePair<string, string>(typeof(Evaluatable).Namespace!, $"{typeof(Evaluatable).Namespace}.Models");
    }
}
