namespace DialogFramework.Domain.ValidationRules;

public partial record ConditionalRequiredValidationRuleBase
{
    public override Result Validate<T>(string id, T value, Dialog dialog) => Result.NotSupported();
}

