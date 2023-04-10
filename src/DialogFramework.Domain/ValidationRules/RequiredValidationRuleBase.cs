namespace DialogFramework.Domain.ValidationRules;

public partial record RequiredValidationRuleBase
{
    public override Result Validate<T>(string id, T value, Dialog dialog) => Result.NotSupported();
}

