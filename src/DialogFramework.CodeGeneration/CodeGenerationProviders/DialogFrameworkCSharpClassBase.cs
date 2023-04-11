namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public abstract partial class DialogFrameworkCSharpClassBase : CSharpClassBase
{
    public override bool RecurseOnDeleteGeneratedFiles => false;

    protected override bool CreateCodeGenerationHeader => true;
    protected override bool EnableNullableContext => true;
    protected override Type RecordCollectionType => typeof(IReadOnlyCollection<>);
    protected override Type RecordConcreteCollectionType => typeof(ReadOnlyValueCollection<>);
    protected override string FileNameSuffix => ".template.generated";
    protected override string ProjectName => "DialogFramework";
    protected override Type BuilderClassCollectionType => typeof(IEnumerable<>);
    protected override bool AddBackingFieldsForCollectionProperties => true;
    protected override bool AddPrivateSetters => true;
    protected override ArgumentValidationType ValidateArgumentsInConstructor => ArgumentValidationType.Optional;
    protected override string[] GetModelAbstractBaseTyped() => new[] { nameof(IEditableQuestionDialogPart) };
    protected override string[] GetExternalCustomBuilderTypes() => new[] { nameof(Evaluatable) };

    protected override IEnumerable<KeyValuePair<string, string>> GetCustomBuilderNamespaceMapping()
    {
        yield return new KeyValuePair<string, string>(typeof(Evaluatable).Namespace!, $"{typeof(Evaluatable).Namespace!}.Builders");
    }

    protected override void FixImmutableBuilderProperty(ClassPropertyBuilder property, string typeName)
    {
        if (typeName == typeof(Version).FullName)
        {
            property.SetDefaultValueForBuilderClassConstructor(new Literal($"new {typeof(Version).FullName}(1, 0, 0)"));
        }
        
        base.FixImmutableBuilderProperty(property, typeName);
    }
}
