namespace DialogFramework.Application;

public class DialogService : IDialogService
{
    private readonly IDialogRepository _dialogRepository;
    private readonly IEnumerable<IDialogSubmitter> _submitters;

    public DialogService(IDialogRepository dialogRepository, IEnumerable<IDialogSubmitter> submitters)
    {
        _dialogRepository = dialogRepository;
        _submitters = submitters;
    }

    public Result<Dialog> Submit(Dialog dialog)
    {
        var validationResult = Validate(dialog);
        if (!validationResult.IsSuccessful())
        {
            return Result<Dialog>.FromExistingResult(validationResult, dialog);
        }

        var supportedSubmitters = _submitters.Where(x => x.SupportsDialog(dialog.DefinitionId, dialog.DefinitionVersion)).ToArray();
        if (supportedSubmitters.Length == 0)
        {
            return Result<Dialog>.NotSupported($"The dialog definition Id [{dialog.DefinitionId}], version [{dialog.DefinitionVersion}] is not supported");
        }

        // In case multiple submitters are found, take the last one.
        // Best way is to first register most generic submitters, then more specific, and last the one to indicate the dialog is not supported.
        var submitter = supportedSubmitters.Last();

        return submitter.Submit(dialog);
    }

    public Result Validate(Dialog dialog)
    {
        var definitionResult = _dialogRepository.Get(dialog.DefinitionId, dialog.DefinitionVersion);
        if (!definitionResult.IsSuccessful())
        {
            return definitionResult;
        }

        var definition = definitionResult.GetValueOrThrow();
        var validationErrors = new List<ValidationError>();

        var allPartsResult = definition.GetAllParts();
        if (!allPartsResult.IsSuccessful())
        {
            return allPartsResult;
        }

        var allParts = allPartsResult.GetValueOrThrow();
        foreach (var part in allParts)
        {
            if (part is not IValidatableDialogPart validatableDialogPart)
            {
                continue;
            }
            
            var valueResult = dialog.GetResultValueByPartId(part.Id);
            object? value;
            if (valueResult.Status == ResultStatus.NotFound)
            {
                value = null;
            }
            else if (valueResult.Status == ResultStatus.Ok)
            {
                value = valueResult.Value;
            }
            else
            {
                // something went wrong
                return valueResult;
            }

            var validationResult = validatableDialogPart.Validate(value, dialog);
            if (!validationResult.IsSuccessful())
            {
                return validationResult;
            }

            if (validationResult.Status == ResultStatus.Invalid)
            {
                validationErrors.AddRange(validationResult.ValidationErrors);
            }
        }

        return validationErrors.Any()
            ? Result.Invalid("Validation failed", validationErrors)
            : Result.Success();
    }
}
