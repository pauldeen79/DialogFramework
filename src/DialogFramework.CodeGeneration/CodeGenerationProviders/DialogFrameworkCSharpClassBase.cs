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

    // this override is needed to get inheritance working
    //TODO: Review if we can move this to ModelFramework
    protected override bool IsMemberValid(IParentTypeContainer parent, ITypeBase typeBase)
        => parent != null
        && typeBase != null
        && (string.IsNullOrEmpty(parent.ParentTypeFullName)
            || parent.ParentTypeFullName.GetClassName() == $"I{typeBase.Name}"
            || (BaseClass != null && $"I{typeBase.Name}" == BaseClass.Name));

    // this override is needed to allow ExpressionFramework entities to be used as builders, too
    protected override Dictionary<string, string> GetBuilderNamespaceMappings()
    {
        var result =  base.GetBuilderNamespaceMappings();

        result.Add("ExpressionFramework.Domain", "ExpressionFramework.Domain.Builders");

        return result;
    }

    // this override is needed to allow Evaluatable builders to work, because the builder is an abstract type and needs a factory
    protected override string[] GetCustomBuilderTypes()
        => base.GetCustomBuilderTypes().Concat(new[] { "Evaluatable" }).ToArray();
}
