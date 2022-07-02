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
        foreach (var property in classBuilder.Properties)
        {
            FixProperty(property);
        }

        classBuilder.AddProperties(GetAdditionalProperties(classBuilder.Name));
    }

    protected override void FixImmutableBuilderProperties(InterfaceBuilder interfaceBuilder)
    {
        foreach (var property in interfaceBuilder.Properties)
        {
            FixProperty(property);
        }

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
                .AddParameter(name: "definition", typeName: "DialogFramework.Abstractions.IDialogDefinition");
        }
    }

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
    }

    private static void FixTypeName(ClassPropertyBuilder property)
    {
        var typeName = property.TypeName.FixTypeName();
        if (typeName.StartsWithAny(StringComparison.InvariantCulture, "DialogFramework.Abstractions.I",
                                                                      "DialogFramework.Abstractions.DialogParts.I"))
        {
            property.ConvertSinglePropertyToBuilderOnBuilder
            (
                typeName
                    .Replace("Abstractions.I", "Domain.Builders.", StringComparison.InvariantCulture)
                    .Replace("Abstractions.DialogParts.I", "Domain.DialogParts.Builders.", StringComparison.InvariantCulture)
                    + "Builder",
                property.IsNullable
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
                    argumentType: $"{typeof(List<>).WithoutGenerics()}<DialogFramework.Domain.Builders.DialogPartResultBuilder>",
                    customBuilderConstructorInitializeExpression: "Results.AddRange(source.GetAllResults(definition).Select(x => new DialogPartResultBuilder(x)))");

            yield return new ClassPropertyBuilder()
                .WithName("Properties")
                .WithTypeName($"{typeof(IReadOnlyCollection<>).WithoutGenerics()}<{nameof(IProperty)}>")
                .ConvertCollectionPropertyToBuilderOnBuilder(
                    addNullChecks: true,
                    argumentType: $"{typeof(List<>).WithoutGenerics()}<DialogFramework.Domain.Builders.PropertyBuilder>",
                    customBuilderConstructorInitializeExpression: "Properties.AddRange(source.GetProperties().Select(x => new PropertyBuilder(x)))");
        }

        if (className == "NavigationDialogPart")
        {
            yield return
                new ClassPropertyBuilder()
                    .WithName("NavigateToId")
                    .WithType(typeof(IDialogPartIdentifier))
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderArgumentType, $"DialogFramework.Domain.Builders.DialogPartIdentifierBuilder")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderMethodParameterExpression, $"NavigateToId?.Build()")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, "_navigateToIdDelegate = new (() => new DialogPartIdentifierBuilder())"); //HACK
        }

        if (className == "DecisionDialogPart")
        {
            yield return
                new ClassPropertyBuilder()
                    .WithName("Decisions")
                    .WithTypeName($"{typeof(IReadOnlyCollection<>).WithoutGenerics()}<DialogFramework.Abstractions.IDecision>")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderArgumentType, $"{typeof(ReadOnlyValueCollection<>).WithoutGenerics()}<DialogFramework.Domain.Builders.DecisionBuilder>")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderMethodParameterExpression, $"new {typeof(ReadOnlyValueCollection<>).WithoutGenerics()}<DialogFramework.Abstractions.IDecision>(Decisions.Select(x => x.Build()))")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, "Decisions.AddRange(source.GetDecisionBuilders())"); //HACK
            yield return
                new ClassPropertyBuilder()
                    .WithName("DefaultNextPartId")
                    .WithType(typeof(IDialogPartIdentifier))
                    .WithIsNullable()
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderArgumentType, $"DialogFramework.Domain.Builders.DialogPartIdentifierBuilder")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderMethodParameterExpression, $"DefaultNextPartId?.Build()")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, "_defaultNextPartIdDelegate = new (() => source.GetDefaultNextPartIdBuilder())"); //HACK
        }
    }

    private static string GetClassName(string typeName)
    {
        var name = typeName.GetClassName().Substring(1);
        return typeName.Contains(".DialogParts")
            ? $"DialogFramework.Domain.DialogParts.Builders.{name}"
            : $"DialogFramework.Domain.Builders.{name}";
    }

    protected static Type[] CoreModels => new[]
    {
        typeof(IAfterNavigateArguments),
        typeof(IBeforeNavigateArguments),
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
}
