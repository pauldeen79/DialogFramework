namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public abstract partial class DialogFrameworkCSharpClassBase : CSharpClassBase
{
    protected override bool CreateCodeGenerationHeader => true;
    protected override bool EnableNullableContext => true;
    protected override bool AddNullChecks => true;
    protected override string FileNameSuffix => ".template.generated";
    protected override Type RecordCollectionType => typeof(IReadOnlyCollection<>);

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

        if (instance.Namespace == "DialogFramework.Domain.DomainModel")
        {
            return forCreate
                ? "DialogFramework.Domain.DomainModel." + instance.Name
                : "DialogFramework.Abstractions.DomainModel.I" + instance.Name;
        }

        if (instance.Namespace == "DialogFramework.Domain.DomainModel.DialogParts")
        {
            return forCreate
                ? "DialogFramework.Domain.DomainModel.DialogParts." + instance.Name
                : "DialogFramework.Abstractions.DomainModel.DialogParts.I" + instance.Name;
        }

        return string.Empty;
    }

    protected override void FixImmutableBuilderProperties(ClassBuilder classBuilder)
    {
        foreach (var property in classBuilder.Properties)
        {
            FixTypeName(property);

            if (property.TypeName.GetClassName() == "ResultValueType"
                || property.TypeName.GetClassName() == "DialogState"
                || property.TypeName == typeof(string).FullName)
            {
                property.WithConstructorNullCheck(false);
            }

            if (property.TypeName.GetClassName() == "IDialogPartResultValue")
            {
                property.SetDefaultValueForBuilderClassConstructor(new Literal("new DialogFramework.Domain.DomainModel.DialogPartResultValues.Builders.EmptyDialogPartResultValueBuilder()"));
            }
        }

        AddProperties(classBuilder);
    }

    private static void FixTypeName(ClassPropertyBuilder property)
    {
        var typeName = property.TypeName.FixTypeName();
        if (typeName.StartsWithAny(StringComparison.InvariantCulture, "DialogFramework.Abstractions.DomainModel.I",
                                                                      "DialogFramework.Abstractions.DomainModel.DialogParts.I"))
        {
            property.ConvertSinglePropertyToBuilderOnBuilder
            (
                typeName
                    .Replace("Abstractions.DomainModel.I", "Domain.DomainModel.Builders.", StringComparison.InvariantCulture)
                    .Replace("Abstractions.DomainModel.DialogParts.I", "Domain.DomainModel.DialogParts.Builders.", StringComparison.InvariantCulture)
                    + "Builder",
                property.IsNullable
                    ? $"_{property.Name.ToPascalCase()}Delegate = new(() => source.{property.Name} == null ? default : new {GetClassName(typeName)}Builder(source.{property.Name}))"
                    : $"_{property.Name.ToPascalCase()}Delegate = new(() => new {GetClassName(typeName)}Builder(source.{property.Name}))" //HACK
            );
        }
        else if (typeName.Contains("Collection<DialogFramework."))
        {
            property.ConvertCollectionPropertyToBuilderOnBuilder
            (
                false,
                typeof(ReadOnlyValueCollection<>).WithoutGenerics(),
                typeName
                    .Replace("Abstractions.DomainModel.I", "Domain.DomainModel.Builders.", StringComparison.InvariantCulture)
                    .Replace("Abstractions.DomainModel.DialogParts.I", "Domain.DomainModel.DialogParts.Builders.", StringComparison.InvariantCulture)
                    .ReplaceSuffix(">", "Builder>", StringComparison.InvariantCulture)
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
        else if (typeName.Contains("Collection<System.String"))
        {
            property.AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderMethodParameterExpression, $"new {typeof(ReadOnlyValueCollection<string>).FullName?.FixTypeName()}({{0}})");
        }
        else if (typeName.IsBooleanTypeName() || typeName.IsNullableBooleanTypeName())
        {
            property.SetDefaultArgumentValueForWithMethod(true);
            if (property.Name == "CanStart")
            {
                property.SetDefaultValueForBuilderClassConstructor(new Literal("true"));
            }
        }
    }

    private static void AddProperties(ClassBuilder classBuilder)
    {
        if (classBuilder.Name == "DialogContext")
        {
            classBuilder.AddProperties
            (
                new ClassPropertyBuilder()
                    .WithName("Answers")
                    .WithTypeName($"{typeof(IReadOnlyCollection<>).WithoutGenerics()}<DialogFramework.Abstractions.DomainModel.IDialogPartResult>")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderMethodParameterExpression, $"new {typeof(ReadOnlyValueCollection<>).WithoutGenerics()}<DialogFramework.Abstractions.DomainModel.IDialogPartResult>(Answers)")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, "Answers = new List<IDialogPartResult>((source as DialogContext)?.Answers ?? Enumerable.Empty<IDialogPartResult>())"), //HACK
                new ClassPropertyBuilder()
                    .WithName("Exception")
                    .WithType(typeof(Exception))
                    .WithIsNullable()
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, "_exceptionDelegate = new (() => default)") //HACK
            );
        }

        if (classBuilder.Name == "NavigationDialogPart")
        {
            classBuilder.AddProperties
            (
                new ClassPropertyBuilder()
                    .WithName("NavigateToId")
                    .WithType(typeof(string))
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, "_navigateToIdDelegate = new (() => string.Empty)") //HACK
            );
        }

        if (classBuilder.Name == "DecisionDialogPart")
        {
            classBuilder.AddProperties
            (
                new ClassPropertyBuilder()
                    .WithName("Decisions")
                    .WithTypeName($"{typeof(IReadOnlyCollection<>).WithoutGenerics()}<DialogFramework.Abstractions.DomainModel.IDecision>")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderArgumentType, $"{typeof(ReadOnlyValueCollection<>).WithoutGenerics()}<DialogFramework.Domain.DomainModel.Builders.DecisionBuilder>")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderMethodParameterExpression, $"new {typeof(ReadOnlyValueCollection<>).WithoutGenerics()}<DialogFramework.Abstractions.DomainModel.IDecision>(Decisions.Select(x => x.Build()))")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, "// skip: Decisions"), //HACK
                new ClassPropertyBuilder()
                    .WithName("DefaultNextPartId")
                    .WithType(typeof(string))
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, "_defaultNextPartIdDelegate = new (() => string.Empty)") //HACK
            );
        }
    }

    private static string GetClassName(string typeName)
    {
        var name = typeName.GetClassName().Substring(1);
        return typeName.Contains(".DialogParts")
            ? $"DialogFramework.Domain.DomainModel.DialogParts.Builders.{name}"
            : $"DialogFramework.Domain.DomainModel.Builders.{name}";
    }
}
