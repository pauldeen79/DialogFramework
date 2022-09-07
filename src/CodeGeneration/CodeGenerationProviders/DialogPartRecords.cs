namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartRecords : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogParts";
    public override string DefaultFileName => "Entities.template.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override IClass? BaseClass => CreateBaseclass(typeof(IDialogPart), "DialogFramework.Domain");

    public override object CreateModel()
        => GetImmutableClasses(DialogPartModels, "DialogFramework.Domain.DialogParts")
        .OfType<IClass>()
        .Select(x => new ClassBuilder(x).With(y => PostProcessImmutableEntityClass(y)).Build())
        .ToArray();
}
