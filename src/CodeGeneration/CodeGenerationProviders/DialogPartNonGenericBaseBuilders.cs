namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartNonGenericBaseBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogParts/Builders";
    public override string DefaultFileName => "Builders.template.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override string FileNameSuffix => ".nongeneric.template.generated";

    public override object CreateModel()
        => GetImmutableNonGenericBuilderClasses(DialogPartBaseModels,
                                                "DialogFramework.Domain.DialogParts",
                                                "DialogFramework.Domain.DialogParts.Builders")
        .OfType<IClass>()
        .Select(x => new ClassBuilder(x).With(y => PostProcessImmutableBuilderClass(y)).Build())
        .ToArray();
}
