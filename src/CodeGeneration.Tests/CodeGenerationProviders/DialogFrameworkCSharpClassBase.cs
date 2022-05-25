namespace CodeGeneration.Tests.CodeGenerationProviders;

public abstract partial class DialogFrameworkCSharpClassBase : CSharpClassBase
{
    protected override bool CreateCodeGenerationHeader => true;
    protected override bool EnableNullableContext => true;
    protected override bool AddNullChecks => true;
    protected override Type RecordCollectionType => typeof(ValueCollection<>);

    protected override string FormatInstanceTypeName(ITypeBase instance, bool forCreate)
    {
        if (instance == null)
        {
            // Not possible, but needs to be added because TTTF.Runtime doesn't support nullable reference types
            return string.Empty;
        }

        if (instance.Namespace == "DialogFramework.Core")
        {
            return forCreate
                ? "DialogFramework.Core." + instance.Name
                : "DialogFramework.Abstractions.I" + instance.Name;
        }

        if (instance.Namespace == "DialogFramework.Core.DomainModel")
        {
            return forCreate
                ? "DialogFramework.Core.DomainModel." + instance.Name
                : "DialogFramework.Abstractions.DomainModel.I" + instance.Name;
        }

        if (instance.Namespace == "DialogFramework.Core.DomainModel.DialogParts")
        {
            return forCreate
                ? "DialogFramework.Core.DomainModel.DialogParts." + instance.Name
                : "DialogFramework.Abstractions.DomainModel.DialogParts.I" + instance.Name;
        }

        return string.Empty;
    }

    protected override void FixImmutableBuilderProperties(ClassBuilder classBuilder)
    {
        if (classBuilder == null)
        {
            // Not possible, but needs to be added because TTTF.Runtime doesn't support nullable reference types
            return;
        }

        foreach (var property in classBuilder.Properties)
        {
            var typeName = property.TypeName.FixTypeName();
            if (typeName.StartsWithAny(StringComparison.InvariantCulture, "DialogFramework.Abstractions.DomainModel.I",
                                                                          "DialogFramework.Abstractions.DomainModel.DialogParts.I"))
            {
                property.ConvertSinglePropertyToBuilderOnBuilder
                (
                    typeName
                        .Replace("Abstractions.DomainModel.I", "Core.DomainModel.Builders.", StringComparison.InvariantCulture)
                        .Replace("Abstractions.DomainModel.DialogParts.I", "Core.DomainModel.DialogParts.Builders.", StringComparison.InvariantCulture)
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
                    typeof(ValueCollection<>).WithoutGenerics(),
                    typeName
                        .Replace("Abstractions.DomainModel.I", "Core.DomainModel.Builders.", StringComparison.InvariantCulture)
                        .Replace("Abstractions.DomainModel.DialogParts.I", "Core.DomainModel.DialogParts.Builders.", StringComparison.InvariantCulture)
                        .ReplaceSuffix(">", "Builder>", StringComparison.InvariantCulture)
                );
            }
            else if (typeName.Contains("Collection<ExpressionFramework."))
            {
                property.ConvertCollectionPropertyToBuilderOnBuilder
                (
                    false,
                    typeof(ValueCollection<>).WithoutGenerics(),
                    typeName
                        .Replace("ExpressionFramework.Abstractions.DomainModel.I", "ExpressionFramework.Core.DomainModel.Builders.", StringComparison.InvariantCulture)
                        .ReplaceSuffix(">", "Builder>", StringComparison.InvariantCulture)
                );
            }
            else if (typeName.Contains("Collection<System.String"))
            {
                property.AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderMethodParameterExpression, $"new {typeof(ValueCollection<string>).FullName?.FixTypeName()}({{0}})");
            }
            else if (typeName.IsBooleanTypeName() || typeName.IsNullableBooleanTypeName())
            {
                property.SetDefaultArgumentValueForWithMethod(true);
                if (property.Name == nameof(IDialogMetadata.CanStart))
                {
                    property.SetDefaultValueForBuilderClassConstructor(new Literal("true"));
                }
            }
            
            if (property.TypeName == typeof(ResultValueType).FullName
                || property.TypeName == typeof(DialogState).FullName
                || property.TypeName == typeof(string).FullName)
            {
                property.WithConstructorNullCheck(false);
            }

            if (property.TypeName == typeof(IDialogPartResultValue).FullName)
            {
                property.SetDefaultValueForBuilderClassConstructor(new Literal("new DialogFramework.Core.DomainModel.DialogPartResultValues.Builders.EmptyDialogPartResultValueBuilder()"));
            }
        }

        if (classBuilder.Name == "DialogContext")
        {
            classBuilder.AddProperties
            (
                new ClassPropertyBuilder()
                    .WithName("Answers")
                    .WithTypeName($"{typeof(ValueCollection<>).WithoutGenerics()}<{typeof(IDialogPartResult).FullName!}>")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderMethodParameterExpression, $"new {typeof(ValueCollection<>).WithoutGenerics()}<{typeof(IDialogPartResult).FullName!}>(Answers)")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, "// skip: Answers"), //HACK
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
                    .WithTypeName($"{typeof(ValueCollection<>).WithoutGenerics()}<{typeof(IDecision).FullName!}>")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderArgumentType, $"{typeof(ValueCollection<>).WithoutGenerics()}<DialogFramework.Core.DomainModel.Builders.DecisionBuilder>")
                    .AddMetadata(ModelFramework.Objects.MetadataNames.CustomBuilderMethodParameterExpression, $"new {typeof(ValueCollection<>).WithoutGenerics()}<{typeof(IDecision).FullName!}>(Decisions.Select(x => x.Build()))")
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
            ? $"DialogFramework.Core.DomainModel.DialogParts.Builders.{name}"
            : $"DialogFramework.Core.DomainModel.Builders.{name}";
    }
}
