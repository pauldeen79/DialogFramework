namespace DialogFramework.CodeGeneration.Models.DialogPartResults;

public interface ISingleQuestionDialogPartResult<out T> : IDialogPartResult
{
    T? Value { get; }
}
