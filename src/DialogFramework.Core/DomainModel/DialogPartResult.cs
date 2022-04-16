﻿namespace DialogFramework.Core.DomainModel;

public record DialogPartResult : IDialogPartResult
{
    /// <summary>
    /// Constructs a dialog part result from a non-question dialog part.
    /// </summary>
    public DialogPartResult(IDialogPart dialogPart)
        : this(dialogPart, new NonQuestionDialogPartResult(), new EmptyDialogPartResultValue())
    {
    }

    /// <summary>
    /// Constructs a dialog part result from a question dialog part, without a value.
    /// </summary>
    public DialogPartResult(IDialogPart dialogPart,
                            IQuestionDialogPartResult answer)
        : this(dialogPart, answer, new EmptyDialogPartResultValue())
    {
    }

    /// <summary>
    /// Constructs a dialog part result from a question dialog part, with a value.
    /// </summary>
    public DialogPartResult(IDialogPart dialogPart,
                            IQuestionDialogPartResult answer,
                            IDialogPartResultValue value)
    {
        DialogPart = dialogPart;
        Result = answer;
        Value = value;
    }

    public IDialogPart DialogPart { get; }
    public IQuestionDialogPartResult Result { get; }
    public IDialogPartResultValue Value { get; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var questionDialogPart = DialogPart as IQuestionDialogPart;
        if (questionDialogPart != null && !questionDialogPart.Results.Any(a => a.Id == Result.Id))
        {
            yield return new ValidationResult($"Unknown result: [{Result.Id}]");
        }
    }
}