namespace DialogFramework.CodeGeneration.Models;

public interface IClosedQuestionOption
{
    [ValidateObject] Evaluatable? Condition { get; }
    [Required] string Key { get; }
    [Required] string DisplayName { get; }
}
