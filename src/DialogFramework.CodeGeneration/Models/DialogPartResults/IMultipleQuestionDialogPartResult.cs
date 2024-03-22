namespace DialogFramework.CodeGeneration.Models.DialogPartResults;

public interface IMultipleQuestionDialogPartResult<out T> : IDialogPartResult
{
    [Required][ValidateObject] IReadOnlyCollection<T> Values { get; }
}
