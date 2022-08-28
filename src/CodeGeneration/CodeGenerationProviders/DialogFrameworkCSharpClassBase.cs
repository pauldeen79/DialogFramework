namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public abstract partial class DialogFrameworkCSharpClassBase : CSharpClassBase
{
    protected override bool CreateCodeGenerationHeader => true;
    protected override bool EnableNullableContext => true;
    protected override bool AddNullChecks => true;
    protected override string FileNameSuffix => ".template.generated";
    protected override Type RecordCollectionType => typeof(IReadOnlyCollection<>);
    protected override bool AddPrivateSetters => true;
    protected override bool CopyPropertyCode => false;

    protected override bool IsMemberValid(IParentTypeContainer parent, ITypeBase typeBase)
        =>
        (
            string.IsNullOrEmpty(parent.ParentTypeFullName)
            || parent.ParentTypeFullName.GetClassName() == $"I{typeBase.Name}"
            || parent.ParentTypeFullName.GetClassName() == nameof(IGroupedDialogPart)
        )
        && typeBase.Name != nameof(IDialogPart);

    protected override string FormatInstanceTypeName(ITypeBase instance, bool forCreate)
    {
        if (instance == null)
        {
            // Not possible, but needs to be added because TTTF.Runtime doesn't support nullable reference types
            return string.Empty;
        }

        if (instance.Namespace == "DialogFramework.Domain")
        {
            return forCreate
                ? "DialogFramework.Domain." + instance.Name
                : "DialogFramework.Abstractions.I" + instance.Name;
        }

        if (instance.Namespace == "DialogFramework.Domain.DialogParts")
        {
            return forCreate
                ? "DialogFramework.Domain.DialogParts." + instance.Name
                : "DialogFramework.Abstractions.DialogParts.I" + instance.Name;
        }

        return string.Empty;
    }

    protected override void FixImmutableBuilderProperties<TBuilder, TEntity>(TypeBaseBuilder<TBuilder, TEntity> typeBaseBuilder)
    {
        typeBaseBuilder.Properties.ForEach(FixProperty);
        typeBaseBuilder.AddProperties(GetAdditionalProperties(typeBaseBuilder.Name));
    }

    protected override void FixImmutableClassProperties<TBuilder, TEntity>(TypeBaseBuilder<TBuilder, TEntity> typeBaseBuilder)
        => FixImmutableBuilderProperties(typeBaseBuilder);

    private static void FixProperty(ClassPropertyBuilder property)
    {
        FixTypeName(property);

        if (property.TypeName.GetClassName() == nameof(ResultValueType)
            || property.TypeName.GetClassName() == nameof(DialogState)
            || property.TypeName == typeof(string).FullName)
        {
            property.WithConstructorNullCheck(false);
        }

        if (property.TypeName.GetClassName() == nameof(IDialogPartResultValueAnswer))
        {
            property.SetDefaultValueForBuilderClassConstructor(new Literal($"new DialogFramework.Domain.Builders.{typeof(IDialogPartResultValueAnswer).GetEntityClassName()}Builder()"));
        }
    }

    private static void FixTypeName(ClassPropertyBuilder property)
    {
        var typeName = property.TypeName.FixTypeName();
        if (typeName.StartsWithAny(StringComparison.InvariantCulture, "DialogFramework.Abstractions.I",
                                                                      "DialogFramework.Abstractions.DialogParts.I"))
        {
            property.ConvertSinglePropertyToBuilderOnBuilder
            (
                argumentType: typeName
                    .Replace("Abstractions.I", "Domain.Builders.", StringComparison.InvariantCulture)
                    .Replace("Abstractions.DialogParts.I", "Domain.DialogParts.Builders.", StringComparison.InvariantCulture)
                    + "Builder",
                customBuilderConstructorInitializeExpression: property.IsNullable
                    ? $"_{property.Name.ToPascalCase()}Delegate = new(() => source.{property.Name} == null ? default : new {GetClassName(typeName)}Builder(source.{property.Name}))"
                    : $"_{property.Name.ToPascalCase()}Delegate = new(() => new {GetClassName(typeName)}Builder(source.{property.Name}))" //HACK
            );
        }
        else if (typeName.Contains("Collection<DialogFramework."))
        {
            var isDialogPart = typeName.Contains($"{typeof(IDialogPart).FullName}>");
            property.ConvertCollectionPropertyToBuilderOnBuilder
            (
                false,
                typeof(ReadOnlyValueCollection<>).WithoutGenerics(),
                isDialogPart
                    ? typeName.ReplaceSuffix(">", "Builder>", StringComparison.InvariantCulture)
                    : typeName.Replace("Abstractions.I", "Domain.Builders.", StringComparison.InvariantCulture).ReplaceSuffix(">", "Builder>", StringComparison.InvariantCulture),
                isDialogPart
                    ? "{4}{0}.AddRange(source.{0}.Select(x => x.CreateBuilder()))"
                    : null
            );
        }
        else if (typeName.Contains("Collection<ExpressionFramework."))
        {
            property.ConvertCollectionPropertyToBuilderOnBuilder
            (
                false,
                typeof(ReadOnlyValueCollection<>).WithoutGenerics(),
                typeName
                    .Replace("ExpressionFramework.Abstractions.DomainModel.I", "ExpressionFramework.Core.DomainModel.Builders.", StringComparison.InvariantCulture)
                    .ReplaceSuffix(">", "Builder>", StringComparison.InvariantCulture)
            );
        }
        else if (typeName.IsBooleanTypeName() || typeName.IsNullableBooleanTypeName())
        {
            property.SetDefaultArgumentValueForWithMethod(true);
            if (property.Name == nameof(IDialogMetadata.CanStart))
            {
                property.SetDefaultValueForBuilderClassConstructor(new Literal("true"));
            }
        }
    }

    private static IEnumerable<ClassPropertyBuilder> GetAdditionalProperties(string className)
    {
        if (className == typeof(IDialog).GetEntityClassName())
        {
            yield return new ClassPropertyBuilder()
                .WithName("Results")
                .WithTypeName($"{typeof(IReadOnlyCollection<>).WithoutGenerics()}<{nameof(IDialogPartResult)}>")
                .ConvertCollectionPropertyToBuilderOnBuilder(
                    addNullChecks: true,
                    argumentType: $"{typeof(ReadOnlyValueCollection<>).WithoutGenerics()}<DialogFramework.Domain.Builders.{typeof(IDialogPartResult).GetEntityClassName()}Builder>",
                    collectionType: $"{typeof(ReadOnlyValueCollection<>).WithoutGenerics()}",
                    customBuilderConstructorInitializeExpression: $"Results.AddRange(source.GetAllResults(definition).Select(x => new {typeof(IDialogPartResult).GetEntityClassName()}Builder(x)))")
                .AddGetterLiteralCodeStatements("return _results;")
                .AddSetterLiteralCodeStatements($"_results = new {typeof(ValueCollection<>).WithoutGenerics()}<{nameof(IDialogPartResult)}>(value);");

            yield return new ClassPropertyBuilder()
                .WithName("Properties")
                .WithTypeName($"{typeof(IReadOnlyCollection<>).WithoutGenerics()}<{nameof(IProperty)}>")
                .ConvertCollectionPropertyToBuilderOnBuilder(
                    addNullChecks: true,
                    argumentType: $"{typeof(ReadOnlyValueCollection<>).WithoutGenerics()}<DialogFramework.Domain.Builders.{typeof(IProperty).GetEntityClassName()}Builder>",
                    collectionType: $"{typeof(ReadOnlyValueCollection<>).WithoutGenerics()}",
                    customBuilderConstructorInitializeExpression: $"Properties.AddRange(source.GetProperties().Select(x => new {typeof(IProperty).GetEntityClassName()}Builder(x)))")
                .AddGetterLiteralCodeStatements("return _properties;")
                .AddSetterLiteralCodeStatements($"_properties = new {typeof(ValueCollection<>).WithoutGenerics()}<{nameof(IProperty)}>(value);");
        }

        if (className == typeof(INavigationDialogPart).GetEntityClassName())
        {
            yield return
                new ClassPropertyBuilder()
                    .WithName("NavigateToId")
                    .WithType(typeof(IDialogPartIdentifier))
                    .ConvertSinglePropertyToBuilderOnBuilder
                    (
                        argumentType: $"DialogFramework.Domain.Builders.{typeof(IDialogPartIdentifier).GetEntityClassName()}Builder",
                        customBuilderMethodParameterExpression: $"NavigateToId?.Build()",
                        customBuilderConstructorInitializeExpression: $"_navigateToIdDelegate = new (() => new {typeof(IDialogPartIdentifier).GetEntityClassName()}Builder())"
                    );
        }

        if (className == typeof(IDecisionDialogPart).GetEntityClassName())
        {
            yield return
                new ClassPropertyBuilder()
                    .WithName("Decisions")
                    .WithTypeName($"{typeof(IReadOnlyCollection<>).WithoutGenerics()}<{typeof(IDecision).FullName}>")
                    .ConvertCollectionPropertyToBuilderOnBuilder
                    (
                        addNullChecks: true,
                        collectionType: typeof(ReadOnlyValueCollection<>).WithoutGenerics(),
                        argumentType: $"{typeof(ReadOnlyValueCollection<>).WithoutGenerics()}<DialogFramework.Domain.Builders.{typeof(IDecision).GetEntityClassName()}Builder >",
                        customBuilderConstructorInitializeExpression: $"Decisions.AddRange(source.Get{typeof(IDecision).GetEntityClassName()}Builders())"
                    );
            yield return
                new ClassPropertyBuilder()
                    .WithName("DefaultNextPartId")
                    .WithType(typeof(IDialogPartIdentifier))
                    .WithIsNullable()
                    .ConvertSinglePropertyToBuilderOnBuilder
                    (
                        argumentType: $"DialogFramework.Domain.Builders.{typeof(IDialogPartIdentifier).GetEntityClassName()}Builder",
                        customBuilderMethodParameterExpression: $"DefaultNextPartId?.Build()",
                        customBuilderConstructorInitializeExpression: "_defaultNextPartIdDelegate = new (() => source.GetDefaultNextPartIdBuilder())"
                    );
        }
    }

    private static string GetClassName(string typeName)
    {
        var name = typeName.GetClassName().Substring(1);
        return typeName.Contains(".DialogParts")
            ? $"DialogFramework.Domain.DialogParts.Builders.{name}"
            : $"DialogFramework.Domain.Builders.{name}";
    }

    protected IClass GetDialogPartBaseClass()
        => typeof(IDialogPart).ToClass(new ClassSettings()).ToImmutableClassBuilder(new ImmutableClassSettings
            (
                newCollectionTypeName: RecordCollectionType.WithoutGenerics(),
                constructorSettings: new ImmutableClassConstructorSettings(
                    validateArguments: ValidateArgumentsInConstructor,
                    addNullChecks: AddNullChecks),
                addPrivateSetters: AddPrivateSetters)
            ).WithNamespace("DialogFramework.Domain.DialogParts").WithName(typeof(IDialogPart).GetEntityClassName())
            .With(x => FixImmutableClassProperties(x))
            .Build();

    protected static Type[] CoreModels => new[]
    {
        typeof(IDecision),
        typeof(IDialogDefinition),
        typeof(IDialog),
        typeof(IDialogIdentifier),
        typeof(IDialogDefinitionIdentifier),
        typeof(IDialogMetadata),
        typeof(IDialogPartGroup),
        typeof(IDialogPartGroupIdentifier),
        typeof(IDialogPartIdentifier),
        typeof(IDialogPartResult),
        typeof(IDialogPartResultAnswer),
        typeof(IDialogPartResultAnswerDefinition),
        typeof(IDialogPartResultIdentifier),
        typeof(IDialogPartResultValue),
        typeof(IDialogPartResultValueAnswer),
        typeof(IDialogValidationResult),
        typeof(IError),
        typeof(IProperty),
    };

    protected static Type[] DialogPartModels => new[]
    {
        typeof(IAbortedDialogPart),
        typeof(ICompletedDialogPart),
        typeof(IDecisionDialogPart),
        typeof(IErrorDialogPart),
        typeof(IMessageDialogPart),
        typeof(INavigationDialogPart),
        typeof(IQuestionDialogPart),
        typeof(IRedirectDialogPart),
    };

    protected static Type[] DialogPartBaseModels => new[]
    {
        typeof(IDialogPart)
    };
}
