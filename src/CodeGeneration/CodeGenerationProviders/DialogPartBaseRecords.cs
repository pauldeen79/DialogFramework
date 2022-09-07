namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartBaseRecords : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogParts";
    public override string DefaultFileName => "Entities.template.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;

    public override object CreateModel()
        => GetImmutableClasses(DialogPartBaseModels, "DialogFramework.Domain.DialogParts")
        .OfType<IClass>()
        .Select(x => new ClassBuilder(x).With(y => PostProcessImmutableEntityClass(y)).Build())
        .ToArray();
}
