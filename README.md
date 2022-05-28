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
    .BuildServiceProvider();
var dialog = provider.GetRequiredService<IDialogRepository>().GetDialog(new DialogIdentifier("SimpleFormFlowDialog", "1.0.0"))!;
var sut = provider.GetRequiredService<IDialogService>();

var context = sut.Start(dialog.Metadata);
context.CurrentPart.Id.Should().Be("ContactInfo");
context = sut.Continue
(
    context,
    new DialogPartResultBuilder()
        .WithDialogPartId(context.CurrentPart.Id)
        .WithResultId("EmailAddress")
        .WithValue(new TextDialogPartResultValueBuilder().WithValue("email@address.com"))
        .Build(),
    new DialogPartResultBuilder()
        .WithDialogPartId(context.CurrentPart.Id)
        .WithResultId("TelephoneNumber")
        .WithValue(new TextDialogPartResultValueBuilder().WithValue("911"))
        .Build()
); // ContactInfo -> Newsletter
context = sut.Continue
(
    context,
    new DialogPartResultBuilder()
        .WithDialogPartId(context.CurrentPart.Id)
        .WithResultId("SignUpForNewsletter")
        .WithValue(new YesNoDialogPartResultValueBuilder().WithValue(false))
        .Build()
); // Newsletter -> Completed
```

See unit tests for more examples.

# Project structure

The solution consists of the following projects:
- DialogFramework.Abstractions [TODO: Remove, merge into CodeGeneration]
- CodeGeneration: Interfaces and code generation for domain entity models and builders
- DialogFramework.Domain: Domain entity models and builders
- DialogFramework.Application: Commands and command handlers for executing commands [TODO: Refactor Service into commands]

# TODOs

- Add Answers to IDialogContext, as IReadOnlyCollection of course
- Change CurrentPart on IDialogContext to CurrentPartId, as string