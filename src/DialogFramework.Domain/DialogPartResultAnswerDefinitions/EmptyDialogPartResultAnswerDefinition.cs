﻿namespace DialogFramework.Domain.DialogPartResultAnswerDefinitions;

public record EmptyDialogPartResultAnswerDefinition : IDialogPartResultAnswerDefinition
{
    public IDialogPartResultIdentifier Id => new DialogPartResultIdentifier(string.Empty);
    public string Title => string.Empty;
    public ResultValueType ValueType => ResultValueType.None;

    public IReadOnlyCollection<IDialogPartResultAnswerDefinitionValidator> Validators
        => new ReadOnlyValueCollection<IDialogPartResultAnswerDefinitionValidator>();

    public IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                         IDialogDefinition definition,
                                                         IDialogPart part,
                                                         IEnumerable<IDialogPartResultAnswer> answers)
        => Enumerable.Empty<IDialogValidationResult>();
}