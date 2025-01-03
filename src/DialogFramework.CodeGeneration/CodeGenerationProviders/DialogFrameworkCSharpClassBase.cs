﻿namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public abstract class DialogFrameworkCSharpClassBase(IPipelineService pipelineService) : CsharpClassGeneratorPipelineCodeGenerationProviderBase(pipelineService)
{
    private const string TypeNameDotClassNameBuilder = "{ClassName(NoGenerics($property.TypeName))}Builder";

    public override bool RecurseOnDeleteGeneratedFiles => false;
    public override string LastGeneratedFilesFilename => string.Empty;
    public override Encoding Encoding => Encoding.UTF8;

    protected override string ProjectName => Constants.ProjectName;
    protected override string CoreNamespace => $"{ProjectName}.Domain";
    protected override bool AddBackingFields => true;
    protected override bool CopyAttributes => true;
    protected override bool GenerateMultipleFiles => false;
    protected override bool CreateAsObservable => true;
    protected override SubVisibility SetterVisibility => SubVisibility.Private;
    protected override bool EnableGlobalUsings => true;

    protected override bool IsAbstractType(Type type)
    {
        type = type.IsNotNull(nameof(type));

        if (type.IsInterface && type.Namespace == $"{CodeGenerationRootNamespace}.Models" && type.Name.In(nameof(IDialogPartResult), nameof(IDialogPart), nameof(IValidationRule)))
        {
            return true;
        }
        return base.IsAbstractType(type);
    }

    protected override string[] GetModelAbstractBaseTyped() => [nameof(IEditableQuestionDialogPart)];

    protected override Type EntityCollectionType => typeof(ObservableCollection<>);
    protected override Type EntityConcreteCollectionType => typeof(ObservableCollection<>);
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
