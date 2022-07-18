namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartBuilders : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogParts/Builders";
    public override string DefaultFileName => "Builders.template.generated.cs";
    public override bool RecurseOnDeleteGeneratedFiles => false;
    
    protected override bool EnableInheritance => true;
    protected override IClass? BaseClass => typeof(IDialogPart).ToClass(new ClassSettings()).ToImmutableClassBuilder(new ImmutableClassSettings
        (
            newCollectionTypeName: RecordCollectionType.WithoutGenerics(),
            constructorSettings: new ImmutableClassConstructorSettings(
                validateArguments: ValidateArgumentsInConstructor,
                addNullChecks: AddNullChecks),
            addPrivateSetters: AddPrivateSetters)
        ).WithNamespace("DialogFramework.Domain.DialogParts").WithName("DialogPart")
        .With(x => FixImmutableClassProperties(x))
        .Build();

    public override object CreateModel()
        => GetImmutableBuilderClasses(DialogPartModels,
                                      "DialogFramework.Domain.DialogParts",
                                      "DialogFramework.Domain.DialogParts.Builders");
}
