namespace DialogFramework.CodeGeneration.Models.DialogPartResults;

public interface ISingleQuestionDialogPartResult<out T> : IDialogPartResult
{
    [ValidateObject] T? Value { get; }
}
