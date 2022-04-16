namespace DialogFramework.Core.DomainModel;

public record DialogPartResult : IDialogPartResult
{
    /// <summary>
    /// Constructs a dialog part result from a non-question dialog part.
    /// </summary>
    public DialogPartResult(IDialogPart dialogPart)
        : this(dialogPart, new EmptyDialogPartResultDefinition(), new EmptyDialogPartResultValue())
    {
    }

    /// <summary>
    /// Constructs a dialog part result from a question dialog part, without a result.
    /// </summary>
    public DialogPartResult(IDialogPart dialogPart,
                            IDialogPartResultDefinition result)
        : this(dialogPart, result, new EmptyDialogPartResultValue())
    {
    }

    /// <summary>
    /// Constructs a dialog part result from a question dialog part, with a result.
    /// </summary>
    public DialogPartResult(IDialogPart dialogPart,
                            IDialogPartResultDefinition result,
                            IDialogPartResultValue value)
    {
        DialogPart = dialogPart;
        Result = result;
        Value = value;
    }

    public IDialogPart DialogPart { get; }
    public IDialogPartResultDefinition Result { get; }
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
