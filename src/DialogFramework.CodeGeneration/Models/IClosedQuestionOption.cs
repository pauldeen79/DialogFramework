namespace DialogFramework.CodeGeneration.Models;

public interface IClosedQuestionOption
{
    Evaluatable? Condition { get; }
    [Required]
    string Key { get; }
    [Required]
    string DisplayName { get; }
}
