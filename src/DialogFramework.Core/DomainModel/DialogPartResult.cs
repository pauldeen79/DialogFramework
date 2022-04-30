namespace DialogFramework.Core.DomainModel;

public record DialogPartResult : IDialogPartResult
{
    /// <summary>
    /// Constructs a dialog part result from a non-question dialog part.
    /// </summary>
    public DialogPartResult(string dialogPartId)
        : this(dialogPartId, new EmptyDialogPartResultDefinition().Id, new EmptyDialogPartResultValue())
    {
    }

    /// <summary>
    /// Constructs a dialog part result from a question dialog part id, without a result.
    /// </summary>
    public DialogPartResult(string dialogPartId,
                            string resultId)
        : this(dialogPartId, resultId, new EmptyDialogPartResultValue())
    {
    }

    /// <summary>
    /// Constructs a dialog part result from a question dialog part id, with a result.
    /// </summary>
    public DialogPartResult(string dialogPartId,
                            string resultId,
                            IDialogPartResultValue value)
    {
        DialogPartId = dialogPartId;
        ResultId = resultId;
        Value = value;
    }

    public string DialogPartId { get; }
    public string ResultId { get; }
    public IDialogPartResultValue Value { get; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        => Enumerable.Empty<ValidationResult>();
}
