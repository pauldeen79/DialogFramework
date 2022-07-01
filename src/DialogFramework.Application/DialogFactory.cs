namespace DialogFramework.Application;

public class DialogFactory : IDialogFactory
{
    public Result<IDialog> Create(IDialogDefinition dialogDefinition,
                           IEnumerable<IDialogPartResult> dialogPartResults,
                           IEnumerable<IProperty> properties)
        => Result<IDialog>.Success(new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(dialogDefinition.Metadata))
            .WithCurrentPartId(new DialogPartIdentifierBuilder().WithValue("Empty"))
            .WithCurrentState(DialogState.Initial)
            .AddResults(dialogPartResults.Select(x => new DialogPartResultBuilder(x)))
            .AddProperties(properties.Select(x => new PropertyBuilder(x)))
            .Build());
}
