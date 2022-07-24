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
        => string.IsNullOrEmpty(parent.ParentTypeFullName)
        || parent.ParentTypeFullName.GetClassName() == $"I{typeBase.Name}"
        || parent.ParentTypeFullName.GetClassName() == nameof(IGroupedDialogPart) && typeBase.Name != nameof(IDialogPart);

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

    protected override void FixImmutableBuilderProperties(ClassBuilder classBuilder)
    {
        FixProperties(classBuilder.Properties);
        classBuilder.AddProperties(GetAdditionalProperties(classBuilder.Name));
    }

    protected override void FixImmutableBuilderProperties(InterfaceBuilder interfaceBuilder)
    {
        FixProperties(interfaceBuilder.Properties);
        interfaceBuilder.AddProperties(GetAdditionalProperties(interfaceBuilder.Name));
    }

    protected override void FixImmutableClassProperties(ClassBuilder classBuilder)
        => FixImmutableBuilderProperties(classBuilder);

    protected override void FixImmutableClassProperties(InterfaceBuilder interfaceBuilder)
        => FixImmutableBuilderProperties(interfaceBuilder);

    protected override void PostProcessImmutableBuilderClass(ClassBuilder classBuilder)
    {
        if (classBuilder.Name == "DialogBuilder")
        {
            classBuilder.Constructors
                .Single(x => x.Parameters.Any())
                .AddParameter(name: "definition", type: typeof(IDialogDefinition));
        }

        if (classBuilder.Namespace == "DialogFramework.Domain.DialogParts.Builders")
        {
            if (classBuilder.Name == "DialogPartBuilder")
            {
                // HACK
                classBuilder.Constructors.Single(x => x.Parameters.Count == 1).Parameters.Single().TypeName = typeof(IDialogPart).FullName!;
                classBuilder.GenericTypeArgumentConstraints[0] = $"where TEntity : {typeof(IDialogPart).FullName}";
            }
            else
            {
                // HACK
                classBuilder.BaseClass = $"DialogPartBuilder<{classBuilder.Name}, DialogFramework.Abstractions.DialogParts.I{classBuilder.Name.Replace("Builder", "")}>";
            }
        }
    }

    protected override void PostProcessImmutableEntityClass(ClassBuilder classBuilder)
    {
        if (classBuilder.Namespace == "DialogFramework.Domain.DialogParts"
            && classBuilder.Name != "DialogPart")
        {
            // HACK
            classBuilder.Constructors.Single().WithChainCall("base(id)");
            classBuilder.BaseClass = "DialogPart";
        }
    }

    private static void FixProperties(List<ClassPropertyBuilder> properties)
        => properties.ForEach(FixProperty);

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
            property.SetDefaultValueForBuilderClassConstructor(new Literal("new DialogFramework.Domain.Builders.DialogPartResultValueAnswerBuilder()"));
        }

        if (property.Name == nameof(IBeforeNavigateArguments.Action)
            || property.Name == nameof(IAfterNavigateArguments.Action))
        {
            //HACK: Do not add null check for this value type...
            property.WithConstructorNullCheck(false);
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
        if (className == "Dialog")
        {
            yield return new ClassPropertyBuilder()
                .WithName("Results")
                .WithTypeName($"{typeof(IReadOnlyCollection<>).WithoutGenerics()}<{nameof(IDialogPartResult)}>")
                .ConvertCollectionPropertyToBuilderOnBuilder(
                    addNullChecks: true,
                    argumentType: $"{typeof(ReadOnlyValueCollection<>).WithoutGenerics()}<DialogFramework.Domain.Builders.DialogPartResultBuilder>",
                    collectionType: $"{typeof(ReadOnlyValueCollection<>).WithoutGenerics()}",
                    customBuilderConstructorInitializeExpression: "Results.AddRange(source.GetAllResults(definition).Select(x => new DialogPartResultBuilder(x)))")
                .AddGetterLiteralCodeStatements("return _results;")
                .AddSetterLiteralCodeStatements($"_results = new {typeof(ValueCollection<>).WithoutGenerics()}<{nameof(IDialogPartResult)}>(value);");

            yield return new ClassPropertyBuilder()
                .WithName("Properties")
                .WithTypeName($"{typeof(IReadOnlyCollection<>).WithoutGenerics()}<{nameof(IProperty)}>")
                .ConvertCollectionPropertyToBuilderOnBuilder(
                    addNullChecks: true,
                    argumentType: $"{typeof(ReadOnlyValueCollection<>).WithoutGenerics()}<DialogFramework.Domain.Builders.PropertyBuilder>",
                    collectionType: $"{typeof(ReadOnlyValueCollection<>).WithoutGenerics()}",
                    customBuilderConstructorInitializeExpression: "Properties.AddRange(source.GetProperties().Select(x => new PropertyBuilder(x)))")
                .AddGetterLiteralCodeStatements("return _properties;")
                .AddSetterLiteralCodeStatements($"_properties = new {typeof(ValueCollection<>).WithoutGenerics()}<{nameof(IProperty)}>(value);");
        }

        if (className == "NavigationDialogPart")
        {
            yield return
                new ClassPropertyBuilder()
                    .WithName("NavigateToId")
                    .WithType(typeof(IDialogPartIdentifier))
                    .ConvertSinglePropertyToBuilderOnBuilder
                    (
                        argumentType: $"DialogFramework.Domain.Builders.DialogPartIdentifierBuilder",
                        customBuilderMethodParameterExpression: $"NavigateToId?.Build()",
                        customBuilderConstructorInitializeExpression: "_navigateToIdDelegate = new (() => new DialogPartIdentifierBuilder())"
                    );
        }

        if (className == "DecisionDialogPart")
        {
            yield return
                new ClassPropertyBuilder()
                    .WithName("Decisions")
                    .WithTypeName($"{typeof(IReadOnlyCollection<>).WithoutGenerics()}<{typeof(IDecision).FullName}>")
                    .ConvertCollectionPropertyToBuilderOnBuilder
                    (
                        addNullChecks: true,
                        collectionType: typeof(ReadOnlyValueCollection<>).WithoutGenerics(),
                        argumentType: $"{typeof(ReadOnlyValueCollection<>).WithoutGenerics()}<DialogFramework.Domain.Builders.DecisionBuilder>",
                        customBuilderConstructorInitializeExpression: "Decisions.AddRange(source.GetDecisionBuilders())"
                    );
            yield return
                new ClassPropertyBuilder()
                    .WithName("DefaultNextPartId")
                    .WithType(typeof(IDialogPartIdentifier))
                    .WithIsNullable()
                    .ConvertSinglePropertyToBuilderOnBuilder
                    (
                        argumentType: $"DialogFramework.Domain.Builders.DialogPartIdentifierBuilder",
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
            ).WithNamespace("DialogFramework.Domain.DialogParts").WithName("DialogPart")
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
        typeof(IDialogPartResultDefinition),
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
