﻿namespace CodeGeneration.CodeGenerationProviders;

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
            FixTypeName(property);

            if (property.TypeName.GetClassName() == "ResultValueType"
                || property.TypeName.GetClassName() == "DialogState"
                || property.TypeName == typeof(string).FullName)
            {
                property.WithConstructorNullCheck(false);
            }

            if (property.TypeName.GetClassName() == "IDialogPartResultValue")
            {
                property.SetDefaultValueForBuilderClassConstructor(new Literal("new DialogFramework.Domain.DialogPartResultValues.Builders.EmptyDialogPartResultValueBuilder()"));
            }
        }

        AddProperties(classBuilder);
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
            property.ConvertCollectionPropertyToBuilderOnBuilder
            (
                false,
                typeof(ReadOnlyValueCollection<>).WithoutGenerics(),
                typeName
                    .Replace("Abstractions.I", "Domain.Builders.", StringComparison.InvariantCulture)
                    .Replace("Abstractions.DialogParts.I", "Domain.DialogParts.Builders.", StringComparison.InvariantCulture)
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
        if (classBuilder.Name == "NavigationDialogPart")
        {
            classBuilder.AddProperties
            (
                new ClassPropertyBuilder()
                    .WithName("NavigateToId")
                    .WithTypeName("DialogFramework.Abstractions.IDialogPartIdentifier")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderArgumentType, $"DialogFramework.Domain.Builders.DialogPartIdentifierBuilder")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderMethodParameterExpression, $"NavigateToId.Build()")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, "_navigateToIdDelegate = new (() => new DialogPartIdentifierBuilder())") //HACK
            );
        }

        if (classBuilder.Name == "DecisionDialogPart")
        {
            classBuilder.AddProperties
            (
                new ClassPropertyBuilder()
                    .WithName("Decisions")
                    .WithTypeName($"{typeof(IReadOnlyCollection<>).WithoutGenerics()}<DialogFramework.Abstractions.IDecision>")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderArgumentType, $"{typeof(ReadOnlyValueCollection<>).WithoutGenerics()}<DialogFramework.Domain.Builders.DecisionBuilder>")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderMethodParameterExpression, $"new {typeof(ReadOnlyValueCollection<>).WithoutGenerics()}<DialogFramework.Abstractions.IDecision>(Decisions.Select(x => x.Build()))")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, "Decisions.AddRange(source.GetDecisionBuilders())"), //HACK
                new ClassPropertyBuilder()
                    .WithName("DefaultNextPartId")
                    .WithTypeName("DialogFramework.Abstractions.IDialogPartIdentifier")
                    .WithIsNullable()
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderArgumentType, $"DialogFramework.Domain.Builders.DialogPartIdentifierBuilder")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderMethodParameterExpression, $"DefaultNextPartId?.Build()")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, "_defaultNextPartIdDelegate = new (() => source.GetDefaultNextPartIdBuilder())") //HACK
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
}
