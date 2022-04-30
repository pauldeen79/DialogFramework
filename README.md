# DialogFramework
Framework for creating dialogs (conversations) between a target system and an actor (user or system)

# Usage
The entry point for this framework is the DialogService. You need to have a Dialog instance, and call the Start method on the DialogService with this Dialog instance as argument. This will return a DialogContext. Then, you have to call Continue on the DialogService until the state of the DialogContext is Completed. (which is whenever there is no next part, so the Completed part will be returned)

Besides the Continue method, you also have Abort method on the DialogService to abort the dialog, or the NavigateTo method to navigate to another dialog part.

Each method on the DialogService has a second method to check whether the method can be called in the current state. These methods begin with 'Can', for example CanStart and CanContinue.

# Example
Here is some example C# code, which starts and finishes a one-step dialog:

```C#
var dialog = new SimpleFormFlowDialog();
var factory = new DialogContextFactory();
var service = new DialogService(factory);

var context = service.Start(dialog);
context = service.Continue
(
    context,
    new DialogPartResult
    (
        context.CurrentPart.Id,
        "EmailAddress",
        new TextDialogPartResultValue("email@address.com")
    ),
    new DialogPartResult
    (
        context.CurrentPart.Id,
        "TelephoneNumber",
        new TextDialogPartResultValue("911")
    ),
    new DialogPartResult
    (
        context.CurrentPart.Id,
        "SignUpForNewsletter",
        new YesNoDialogPartResultValue(false)
    )
);
```

See unit tests for more examples.
