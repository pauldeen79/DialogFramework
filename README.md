# DialogFramework
Framework for creating dialogs (conversations) between a target system and an actor (user or system)

# Usage
The entry point for this framework is the DialogService. You need to have a Dialog instance, and call the Start method on the DialogService with this Dialog instance as argument. This will return a DialogContext. Then, you have to call Continue on the DialogService until the state of the DialogContext is Completed. (which is whenever there is no next part, so the Completed part will be returned)

Besides the Continue method, you also have Abort method on the DialogService to abort the dialog, or the NavigateTo method to navigate to another dialog part.

Each method on the DialogService has a second method to check whether the method can be called in the current state. These methods begin with 'Can', for example CanStart and CanContinue.

# Example
Here is some example C# code, which starts and finishes a two-step dialog:

```C#
using var provider = new ServiceCollection()
    .AddDialogFramework()
    .AddSingleton<IDialogRepository, TestDialogRepository>()
    .AddSingleton<ILogger, TestLogger>()
    .BuildServiceProvider();
var dialogDefinition = provider.GetRequiredService<IDialogDefinitionRepository>().GetDialogDefinition(new DialogIdentifier("SimpleFormFlowDialog", "1.0.0"))!;
var sut = provider.GetRequiredService<IDialogService>();

var dialog = sut.Start(dialogDefinition.Metadata);
dialog = sut.Continue
(
    dialog,
    new DialogPartResultBuilder()
        .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
        .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
        .WithValue(new TextDialogPartResultValueBuilder().WithValue("email@address.com"))
        .Build(),
    new DialogPartResultBuilder()
        .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
        .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
        .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
        .Build()
); // ContactInfo -> Newsletter
dialog = sut.Continue
(
    dialog,
    new DialogPartResultBuilder()
        .WithDialogPartId(new DialogPartIdentifierBuilder(context.CurrentPartId))
        .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
        .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(false))
        .Build()
); // Newsletter -> Completed
```

See unit tests for more examples.

# Project structure

The solution consists of the following projects:
- DialogFramework.Abstractions: Interfaces used in code generation
- CodeGeneration: Code generation for domain entity models and builders
- DialogFramework.Domain: Domain entity models and builders
- DialogFramework.Application: Application logic

# TODOs

- Move/copy unit tests from Service (Application) to Domain, including refactor to remove duplication like new Mock<> and using mocks where we can use the real domain model
- Refactor Service into separate request handlers, and implement Mediatr.
- Try to move interfaces from Abstractions to CodeGeneration, and remove references to Abstractions project. Use Domain implementations in signatures instead (inclusing enums, which need to be generated from Abstractions/CodeGeneration). Or, as an alternative, remove manual step from Abstractions.tests to code generation models, by adding project reference and using reflection to get  the model.
