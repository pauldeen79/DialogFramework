﻿namespace DialogFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartResults : DialogFrameworkCSharpClassBase
{
    public override string Path => $"{Constants.Namespaces.Domain}/{nameof(DialogPartResults)}";
    public override string LastGeneratedFilesFileName => string.Empty;

    protected override string FileNameSuffix => string.Empty;
    protected override bool CreateCodeGenerationHeader => false;

    public override object CreateModel()
        => GetOverrideModels(typeof(IDialogPartResult))
            .Select(x => new ClassBuilder()
                .WithNamespace(CurrentNamespace)
                .WithName(x.Name)
                .WithPartial()
                .WithRecord()
                .AddMethods(new ClassMethodBuilder()
                    .WithName("GetValue")
                    .WithOverride()
                    .WithTypeName($"{typeof(Result<>).WithoutGenerics()}<{typeof(object).FullName}?>")
                    .AddNotImplementedException()
                )
                .AddGenericTypeArguments(x.GenericTypeArguments)
                .AddGenericTypeArgumentConstraints(x.GenericTypeArgumentConstraints)
                .Build());

}
