namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public abstract class DialogFrameworkCSharpClassBase : CsharpClassGeneratorPipelineCodeGenerationProviderBase
{
    private const string TypeNameDotClassNameBuilder = "{TypeName.ClassName}Builder";

    protected DialogFrameworkCSharpClassBase(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
    {
    }

    public override bool RecurseOnDeleteGeneratedFiles => false;
    public override string LastGeneratedFilesFilename => string.Empty;
    public override Encoding Encoding => Encoding.UTF8;

    protected override string ProjectName => Constants.ProjectName;
    protected override string CoreNamespace => $"{ProjectName}.Domain";
    protected override bool CreateRecord => false;
    protected override bool AddBackingFields => true;
    protected override bool AddSetters => true;
    protected override SubVisibility SetterVisibility => SubVisibility.Private;

    protected override bool IsAbstractType(Type type)
    {
        type = type.IsNotNull(nameof(type));

        if (type.IsInterface && type.Namespace == $"{CodeGenerationRootNamespace}.Models" && type.Name.In(nameof(IDialogPartResult), nameof(IDialogPart), nameof(IValidationRule)))
        {
            return true;
        }
        return base.IsAbstractType(type);
    }

    protected override string[] GetModelAbstractBaseTyped() => new[] { nameof(IEditableQuestionDialogPart) };

    protected override Type EntityCollectionType => typeof(IReadOnlyCollection<>);
    protected override Type EntityConcreteCollectionType => typeof(ReadOnlyValueCollection<>);
    //protected override Type EntityConcreteCollectionType => typeof(IReadOnlyCollection<>); // when using backing fields, we can't use ReadOnlyValueCollection<> because the value can't be cast on the property setters...
    protected override Type BuilderCollectionType => typeof(ObservableCollection<>);

    protected override IEnumerable<TypenameMappingBuilder> CreateAdditionalTypenameMappings()
        =>
        [
            new TypenameMappingBuilder()
                .WithSourceType(typeof(Evaluatable))
                .WithTargetType(typeof(Evaluatable))
                .AddMetadata
                (
                    new MetadataBuilder().WithValue(typeof(EvaluatableBuilder).Namespace).WithName(ClassFramework.Pipelines.MetadataNames.CustomBuilderNamespace),
                    new MetadataBuilder().WithValue(TypeNameDotClassNameBuilder).WithName(ClassFramework.Pipelines.MetadataNames.CustomBuilderName),
                    //new MetadataBuilder().WithValue("source.{Name}.ToBuilder()").WithName(ClassFramework.Pipelines.MetadataNames.CustomBuilderConstructorInitializeExpression),
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].ToBuilder()[ForcedNullableSuffix]").WithName(ClassFramework.Pipelines.MetadataNames.CustomBuilderSourceExpression),
                    new MetadataBuilder().WithValue(new Literal($"new {typeof(ConstantEvaluatableBuilder).FullName}()", null)).WithName(ClassFramework.Pipelines.MetadataNames.CustomBuilderDefaultValue),
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].Build()").WithName(ClassFramework.Pipelines.MetadataNames.CustomBuilderMethodParameterExpression)
                ),
            new TypenameMappingBuilder()
                .WithSourceType(typeof(Version))
                .WithTargetType(typeof(Version))
                .AddMetadata
                (
                    new MetadataBuilder().WithValue(new Literal($"new {typeof(Version).FullName}(1, 0, 0)", null)).WithName(ClassFramework.Pipelines.MetadataNames.CustomBuilderDefaultValue)
                ),
        ];
}
