namespace DialogFramework.Application;

public class DialogFactory : IDialogFactory
{
    public Result<IDialog> Create(IDialogDefinition definition,
                           IEnumerable<IDialogPartResult> results,
                           IEnumerable<IProperty> properties)
        => Result<IDialog>.Success(new DialogBuilder()
            .WithId(new DialogIdentifierBuilder().WithValue(Guid.NewGuid().ToString()))
            .WithCurrentDialogIdentifier(new DialogDefinitionIdentifierBuilder(definition.Metadata))
            .WithCurrentPartId(new DialogPartIdentifierBuilder().WithValue("Empty"))
            .WithCurrentState(DialogState.Initial)
            .AddResults(results.Select(x => new DialogPartResultBuilder(x)))
            .AddProperties(properties.Select(x => new PropertyBuilder(x)))
            .Build());
}
