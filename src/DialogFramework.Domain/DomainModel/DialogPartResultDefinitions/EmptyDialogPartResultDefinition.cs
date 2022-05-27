﻿namespace DialogFramework.Domain.DomainModel.DialogPartResultDefinitions;

public record EmptyDialogPartResultDefinition : IDialogPartResultDefinition
{
    public string Id => string.Empty;
    public string Title => string.Empty;
    public ResultValueType ValueType => ResultValueType.None;

    public IReadOnlyCollection<IDialogPartResultDefinitionValidator> Validators => new ValueCollection<IDialogPartResultDefinitionValidator>();

    public IEnumerable<IDialogValidationResult> Validate(IDialogContext context,
                                                         IDialog dialog,
                                                         IDialogPart dialogPart,
                                                         IEnumerable<IDialogPartResult> dialogPartResults)
        => Enumerable.Empty<IDialogValidationResult>();
}