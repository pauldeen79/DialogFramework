namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogParts/Builders";
    public override string DefaultFileName => "Builders.template.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override IClass? BaseClass => CreateBaseclass(typeof(IDialogPart), "DialogFramework.Domain");

    public override object CreateModel()
        => GetImmutableBuilderClasses(DialogPartModels,
                                      "DialogFramework.Domain.DialogParts",
                                      "DialogFramework.Domain.DialogParts.Builders")
        .OfType<IClass>()
        .Select(x => new ClassBuilder(x).With(y => PostProcessImmutableBuilderClass(y)).Build())
        .ToArray();
}
