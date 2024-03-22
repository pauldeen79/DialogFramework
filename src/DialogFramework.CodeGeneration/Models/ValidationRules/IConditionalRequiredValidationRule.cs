namespace DialogFramework.CodeGeneration.Models.ValidationRules;

public interface IConditionalRequiredValidationRule : IValidationRule
{
    [Required][ValidateObject] Evaluatable Condition { get; } //note that this type needs to be nullable because of the factory needed to create the builder...
}
