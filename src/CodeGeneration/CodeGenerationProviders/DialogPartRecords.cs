namespace CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class DialogPartRecords : DialogFrameworkCSharpClassBase
{
    public override string Path => "DialogFramework.Domain/DialogParts";
    public override string DefaultFileName => "Entities.template.generated.cs";
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
        => GetImmutableClasses(DialogPartModels, "DialogFramework.Domain.DialogParts");
}
