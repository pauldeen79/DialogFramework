﻿namespace DialogFramework.Abstractions;

public interface IDialog
{
    IDialogIdentifier Id { get; }
    IDialogDefinitionIdentifier CurrentDialogIdentifier { get; }
    IDialogPartIdentifier CurrentPartId { get; }
    IDialogPartGroupIdentifier? CurrentGroupId { get; }
    DialogState CurrentState { get; }
    IReadOnlyCollection<IDialogPartResult> Results { get; }
    IReadOnlyCollection<IDialogValidationResult> ValidationErrors { get; }
    IReadOnlyCollection<IError> Errors { get; }

    Result Start(IDialogDefinition dialogDefinition, IConditionEvaluator conditionEvaluator);

    Result Continue(IDialogDefinition dialogDefinition,
                    IEnumerable<IDialogPartResult> partResults,
                    IConditionEvaluator conditionEvaluator);

    Result Abort(IDialogDefinition dialogDefinition);

    Result Error(IDialogDefinition dialogDefinition, IEnumerable<IError> errors);

    Result NavigateTo(IDialogDefinition dialogDefinition, IDialogPartIdentifier navigateToPartId);

    Result ResetCurrentState(IDialogDefinition dialogDefinition);
}
