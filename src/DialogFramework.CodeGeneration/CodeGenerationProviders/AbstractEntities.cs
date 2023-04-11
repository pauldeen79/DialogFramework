namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractEntities : DialogFrameworkCSharpClassBase
{
    public override string Path => Constants.Namespaces.Domain;

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;

    protected override ArgumentValidationType ValidateArgumentsInConstructor => ArgumentValidationType.Never; // not needed for abstract entities, because each derived class will do its own validation

    public override object CreateModel()
        => GetImmutableClasses(GetAbstractModels(), Constants.Namespaces.Domain);
}
