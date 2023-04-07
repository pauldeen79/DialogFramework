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
        // First validate all submitted data
        var definitionResult = _dialogRepository.Get(dialog.DefinitionId, dialog.DefinitionVersion);
        if (!definitionResult.IsSuccessful())
        {
            return Result<Dialog>.FromExistingResult(definitionResult);
        }
        var definition = definitionResult.GetValueOrThrow();

        var validationErrors = new List<ValidationError>();
        foreach (var dialogPartResult in dialog.Results)
        {
            var partResult = definition.GetPartById(dialogPartResult.PartId);
            if (!partResult.IsSuccessful())
            {
                return Result<Dialog>.FromExistingResult(partResult);
            }
            var part = partResult.GetValueOrThrow();
            if (part is IValidatableDialogPart validatableDialogPart)
            {
                var validationResult = validatableDialogPart.Validate(dialogPartResult.GetValue(), dialog);
                if (validationResult.Status == ResultStatus.Invalid)
                {
                    validationErrors.AddRange(validationResult.ValidationErrors);
                }
                if (!validationResult.IsSuccessful())
                {
                    return Result<Dialog>.FromExistingResult(validationResult);
                }
            }
        }
        if (validationErrors.Any())
        {
            return Result<Dialog>.Invalid("Validation failed", validationErrors);
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
}
