namespace DialogFramework.CodeGeneration.Models.DialogPartResults;

public interface IMultipleQuestionDialogPartResult<out T> : IDialogPartResult
{
    [Required]
    IReadOnlyCollection<T> Values { get; }
}
