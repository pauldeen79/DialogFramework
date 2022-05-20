using CrossCutting.Common;
using DialogFramework.Abstractions;
using DialogFramework.Abstractions.DomainModel;
using DialogFramework.Abstractions.DomainModel.DialogParts;
using DialogFramework.Abstractions.DomainModel.Domains;
using ModelFramework.Common;
using ModelFramework.Common.Extensions;
using ModelFramework.Objects.Builders;
using ModelFramework.Objects.Contracts;
using ModelFramework.Objects.Extensions;

namespace CodeGeneration.Tests.CodeGenerationProviders;

public abstract partial class DialogFrameworkCSharpClassBase : CSharpClassBase
{
    protected override bool CreateCodeGenerationHeader => true;
    protected override bool EnableNullableContext => true;
    protected override bool AddNullChecks => true;
    protected override Type RecordCollectionType => typeof(ValueCollection<>);

    protected IClass[] GetCodeStatementBuilderClasses(Type codeStatementType,
                                                      Type codeStatementInterfaceType,
                                                      Type codeStatementBuilderInterfaceType,
                                                      string buildersNamespace)
        => GetClassesFromSameNamespace(codeStatementType ?? throw new ArgumentNullException(nameof(codeStatementType)))
            .Select
            (
                c => CreateBuilder(c, buildersNamespace)
                    .AddInterfaces(codeStatementBuilderInterfaceType)
                    .Chain(x => x.Methods.First(x => x.Name == "Build").WithType(codeStatementInterfaceType))
                    .Build()
            ).ToArray();

    protected override string FormatInstanceTypeName(ITypeBase instance, bool forCreate)
    {
        if (instance == null)
        {
            // Not possible, but needs to be added because TTTF.Runtime doesn't support nullable reference types
            return string.Empty;
        }

        if (instance.Namespace == "DialogFramework.UniversalModel")
        {
            return forCreate
                ? "DialogFramework.UniversalModel." + instance.Name
                : "DialogFramework.Abstractions.I" + instance.Name;
        }

        if (instance.Namespace == "DialogFramework.UniversalModel.DomainModel")
        {
            return forCreate
                ? "DialogFramework.UniversalModel.DomainModel." + instance.Name
                : "DialogFramework.Abstractions.DomainModel.I" + instance.Name;
        }

        if (instance.Namespace == "DialogFramework.UniversalModel.DomainModel.DialogParts")
        {
            return forCreate
                ? "DialogFramework.UniversalModel.DomainModel.DialogParts." + instance.Name
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
                        .Replace("Abstractions.DomainModel.I", "UniversalModel.DomainModel.Builders.", StringComparison.InvariantCulture)
                        .Replace("Abstractions.DomainModel.DialogParts.I", "UniversalModel.DomainModel.DialogParts.Builders.", StringComparison.InvariantCulture)
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
                        .Replace("Abstractions.DomainModel.I", "UniversalModel.DomainModel.Builders.", StringComparison.InvariantCulture)
                        .Replace("Abstractions.DomainModel.DialogParts.I", "UniversalModel.DomainModel.DialogParts.Builders.", StringComparison.InvariantCulture)
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
            else if (property.Name == nameof(IDialogPart.State))
            {
                property.SetDefaultValueForBuilderClassConstructor(new Literal(GetDefaultValueForDialogState(classBuilder.Name)));
            }
            
            if (property.TypeName == typeof(ResultValueType).FullName
                || property.TypeName == typeof(DialogState).FullName
                || property.TypeName == typeof(string).FullName)
            {
                property.WithConstructorNullCheck(false);
            }
        }

        //if (classBuilder.Name == "DialogPartResultDefinition")
        //{
        //    classBuilder.AddProperties(new ClassPropertyBuilder().WithName("Validators").WithTypeName($"{typeof(ValueCollection<>).WithoutGenerics()}<DialogPartResultDefinitionValidator>"));
        //}

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
    }

    private static string GetDefaultValueForDialogState(string classBuilderName)
        => $"I{classBuilderName}" switch
        {
            nameof(IAbortedDialogPart) => $"{DialogStateName}.{DialogState.Aborted}",
            nameof(ICompletedDialogPart) => $"{DialogStateName}.{DialogState.Completed}",
            nameof(IErrorDialogPart) => $"{DialogStateName}.{DialogState.ErrorOccured}",
            _ => $"{DialogStateName}.{DialogState.InProgress}"
        };

    private static string DialogStateName => typeof(DialogState).FullName!;

    private static string GetClassName(string typeName)
    {
        var name = typeName.GetClassName().Substring(1);
        return typeName.Contains(".DialogParts") //typeName.EndsWith("DialogPart")
            ? $"DialogFramework.UniversalModel.DomainModel.DialogParts.Builders.{name}"
            : $"DialogFramework.UniversalModel.DomainModel.Builders.{name}";
    }

    protected static Type[] GetCoreModelTypes()
        => new[]
        {
            typeof(IDialogContext),
        };

    protected static Type[] GetDomainModelModelTypes()
        => new[]
        {
            typeof(IDialog),
            typeof(IDialogMetadata),
            typeof(IDialogPartGroup),
            typeof(IDialogPartResult),
            typeof(IDialogPartResultDefinition),
            typeof(IDialogPartResultValue),
            typeof(IDialogValidationResult),
        };

    protected static Type[] GetDialogPartModelTypes()
        => new[]
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
