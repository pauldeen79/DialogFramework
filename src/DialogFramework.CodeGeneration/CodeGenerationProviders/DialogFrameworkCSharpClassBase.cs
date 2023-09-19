namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public abstract class DialogFrameworkCSharpClassBase : CSharpClassBase
{
    public override bool RecurseOnDeleteGeneratedFiles => false;
    public override string DefaultFileName => string.Empty; // not used because we're using multiple files, but it's abstract so we need to fill ilt

    protected override bool CreateCodeGenerationHeader => true;
    protected override bool EnableNullableContext => true;
    protected override Type RecordCollectionType => typeof(IReadOnlyCollection<>);
    protected override Type RecordConcreteCollectionType => typeof(ReadOnlyValueCollection<>);
    protected override string FileNameSuffix => Constants.TemplateGenerated;
    protected override string ProjectName => Constants.ProjectName;
    protected override Type BuilderClassCollectionType => typeof(IEnumerable<>);
    protected override bool AddBackingFieldsForCollectionProperties => true;
    protected override bool AddPrivateSetters => true;
    protected override ArgumentValidationType ValidateArgumentsInConstructor => ArgumentValidationType.Shared;
    protected override string[] GetModelAbstractBaseTyped() => new[] { nameof(IEditableQuestionDialogPart) };
    protected override string[] GetExternalCustomBuilderTypes() => new[] { nameof(Evaluatable) };

    protected override IEnumerable<KeyValuePair<string, string>> GetCustomBuilderNamespaceMapping()
    {
        yield return new KeyValuePair<string, string>(typeof(Evaluatable).Namespace!, $"{typeof(Evaluatable).Namespace}.Builders");
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
