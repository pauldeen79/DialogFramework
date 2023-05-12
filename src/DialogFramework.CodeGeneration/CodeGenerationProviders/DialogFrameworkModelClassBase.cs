namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

public abstract class DialogFrameworkModelClassBase : DialogFrameworkCSharpClassBase
{
    protected override string AddMethodNameFormatString => string.Empty; // we don't want Add methods for collection properties
    protected override string SetMethodNameFormatString => string.Empty; // we don't want With methods for non-collection properties
    protected override string BuilderNameFormatString => "{0}Model";
    protected override string BuilderBuildMethodName => "ToEntity";
    protected override string BuilderBuildTypedMethodName => "ToTypedEntity";
    protected override string BuilderName => "Model";
    protected override string BuildersName => "Models";
    protected override bool UseLazyInitialization => false; // we don't want lazy stuff in models, just getters and setters

    protected override IEnumerable<KeyValuePair<string, string>> GetCustomBuilderNamespaceMapping()
    {
        yield return new KeyValuePair<string, string>(typeof(Evaluatable).Namespace!, $"{typeof(Evaluatable).Namespace}.Models");
    }
}
