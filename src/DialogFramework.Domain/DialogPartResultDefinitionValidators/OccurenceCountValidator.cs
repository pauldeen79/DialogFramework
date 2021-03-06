namespace DialogFramework.Domain.DialogPartResultDefinitionValidators;

public class OccurenceCountValidator : IDialogPartResultDefinitionValidator 
{
    private readonly int _minimumOccurenceCount;
    private readonly int _maximumOccurenceCount;

    public OccurenceCountValidator(int requiredCount) : this(requiredCount, requiredCount)
    {
    }

    public OccurenceCountValidator(int minimumOccurenceCount, int maximumOccurenceCount)
    {
        _minimumOccurenceCount = minimumOccurenceCount;
        _maximumOccurenceCount = maximumOccurenceCount;
    }

    public IEnumerable<IDialogValidationResult> Validate(IDialog dialog,
                                                         IDialogDefinition dialogDefinition,
                                                         IDialogPart dialogPart,
                                                         IDialogPartResultDefinition dialogPartResultDefinition,
                                                         IEnumerable<IDialogPartResultAnswer> dialogPartResults)
    {
        var actualCount = dialogPartResults.Count();
        if (actualCount < _minimumOccurenceCount || actualCount > _maximumOccurenceCount)
        {
            var timesName = _minimumOccurenceCount == _maximumOccurenceCount
                ? _minimumOccurenceCount.ToString()
                : $"between {_minimumOccurenceCount} and {_maximumOccurenceCount}";
            var messageSuffix = _minimumOccurenceCount == 1 && _maximumOccurenceCount == 1
                ? "is required"
                : $"should be supplied {timesName} times";
            yield return new DialogValidationResult($"Result value of [{dialogPart.Id}.{dialogPartResultDefinition.Id}] {messageSuffix}", new ReadOnlyValueCollection<IDialogPartResultIdentifier>(new[] { dialogPartResultDefinition.Id }));
        }
    }
}
