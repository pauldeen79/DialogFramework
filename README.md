# DialogFramework
Framework for creating dialogs (conversations) between a target system and an actor (user or system)

# Usage
The entry point for this framework is the DialogApplicationService.
You need to have a DialogDefinition instance, and call the Start method on the DialogService with this Dialog instance as argument. 
This will return a Dialog. Then, you have to call Continue on the DialogApplicationService until the state of the Dialog is Completed. (which is whenever there is no next part, so the Completed part will be returned)

Besides the Continue method, you also have Abort method on the DialogApplicationService to abort the dialog, or the NavigateTo method to navigate to another dialog part.

Each method on the DialogApplicationService has a second method to check whether the method can be called in the current state.
These methods begin with 'Can', for example CanStart and CanContinue.

# Example
Here is some example C# code, which starts and finishes a two-step dialog:

```C#
using var provider = new ServiceCollection()
    .AddDialogFramework()
    .AddSingleton<IDialogDefinitionRepository, TestDialogDefinitionRepository>()
    .AddSingleton<ILogger, TestLogger>()
    .BuildServiceProvider();
var dialogDefinition = _provider.GetRequiredService<IDialogDefinitionRepository>().GetDialogDefinition(new DialogDefinitionIdentifier(nameof(SimpleFormFlowDialog), "1.0.0"))!;
var service = _provider.GetRequiredService<IDialogApplicationService>();

var dialog = sut.Start(dialogDefinition.Metadata).GetValueOrThrow("Start failed");
dialog = sut.Continue
(
    dialog,
    new DialogPartResultBuilder()
        .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
        .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("EmailAddress"))
        .WithValue(new TextDialogPartResultValueBuilder().WithValue("email@address.com"))
        .Build(),
    new DialogPartResultBuilder()
        .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
        .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("TelephoneNumber"))
        .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
        .Build()
).GetValueOrThrow("ContactInfo failed"); // ContactInfo -> Newsletter
dialog = sut.Continue
(
    dialog,
    new DialogPartResultBuilder()
        .WithDialogPartId(new DialogPartIdentifierBuilder(dialog.CurrentPartId))
        .WithResultId(new DialogPartResultIdentifierBuilder().WithValue("SignUpForNewsletter"))
        .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(false))
        .Build()
).GetValueOrThrow("Newsletter failed"); // Newsletter -> Completed
```

See unit tests for more examples.

# Getting local environment up and running

To build the solution on your development machine, you first need to build and run the CodeGeneration project.
This is because I have decided not to commit generated code to the Git repository.

To do this, simply set the CodeGeneration project as start up project in Visual Studio, and hit F5.
Or, alternatively, use dotnet from the command line to build and run it.
Be sure to run it from the root directory, where the solution file resides.
e.g.
dotnet build src/CodeGeneration/CodeGeneration.csproj
dotnet src/CodeGeneration/bin/Debug/net6.0/CodeGeneration.dll

# Project structure

The solution consists of the following projects:
- DialogFramework.Abstractions: Interfaces used in code generation
- CodeGeneration: Code generation for domain entity models and builders
- DialogFramework.Domain: Domain entities and builders
- DialogFramework.Application: Application logic

# TODOs

- Allow interception from outside the dialog and dialogparts, through the AfterNavigate and BeforeNavigate methods
- Try to refactor INavigationDialogPart and IDecisionDialogPart to use AfterNavigate to accomplish this
- Try to move interfaces from Abstractions to CodeGeneration, and remove references to Abstractions project. Use Domain implementations in signatures instead (inclusing enums, which need to be generated from Abstractions/CodeGeneration).
- Create a minimal web api as a demo project, using some sort of state store (local in-memory cache?) for persisting state.