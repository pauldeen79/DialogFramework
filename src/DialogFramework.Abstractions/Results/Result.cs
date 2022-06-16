namespace DialogFramework.Abstractions.Results;

public record Result<T> : Result where T: class
{
    private Result(T? value, ResultStatus status, string? errorMessage, IEnumerable<ValidationError> validationErrors)
        : base(status, errorMessage, validationErrors) => Value = value;
    public T? Value { get; }
    public static Result<T> Success(T value) => new Result<T>(value, ResultStatus.Ok, null, Enumerable.Empty<ValidationError>());
    public static new Result<T> Error(string? errorMessage) => new Result<T>(null, ResultStatus.Error, errorMessage, Enumerable.Empty<ValidationError>());
    public static new Result<T> NotFound(string? errorMessage) => new Result<T>(null, ResultStatus.NotFound, errorMessage, Enumerable.Empty<ValidationError>());
    public static new Result<T> Invalid(IEnumerable<ValidationError> validationErrors) => new Result<T>(null, ResultStatus.Invalid, null, validationErrors);
    public static new Result<T> Invalid(string? errorMessage) => new Result<T>(null, ResultStatus.Invalid, errorMessage, Enumerable.Empty<ValidationError>());
    public static new Result<T> Invalid(string? errorMessage, IEnumerable<ValidationError> validationErrors) => new Result<T>(null, ResultStatus.Invalid, errorMessage, validationErrors);
    public static Result<T> FromExistingResult(Result existingResult) => new Result<T>(null, existingResult.Status, existingResult.ErrorMessage, existingResult.ValidationErrors);

    public T GetValueOrThrow()
    {
        if (!IsSuccessful())
        {
            throw new InvalidOperationException($"Result: {Status}");
        }
        return Value!;
    }
}

public record Result
{
    protected Result(ResultStatus status, string? errorMessage, IEnumerable<ValidationError> validationErrors)
    {
        Status = status;
        ErrorMessage = errorMessage;
        ValidationErrors = new ValueCollection<ValidationError>(validationErrors);
    }

    public bool IsSuccessful() => Status == ResultStatus.Ok;
    public string? ErrorMessage { get; }
    public ResultStatus Status { get; }
    public IReadOnlyCollection<ValidationError> ValidationErrors { get; }

    public static Result Success() => new Result(ResultStatus.Ok, null, Enumerable.Empty<ValidationError>());
    public static Result Error(string? errorMessage) => new Result(ResultStatus.Error, errorMessage, Enumerable.Empty<ValidationError>());
    public static Result NotFound(string? errorMessage) => new Result(ResultStatus.NotFound, errorMessage, Enumerable.Empty<ValidationError>());
    public static Result Invalid(IEnumerable<ValidationError> validationErrors) => new Result(ResultStatus.Invalid, null, validationErrors);
    public static Result Invalid(string? errorMessage) => new Result(ResultStatus.Invalid, errorMessage, Enumerable.Empty<ValidationError>());
    public static Result Invalid(string? errorMessage, IEnumerable<ValidationError> validationErrors) => new Result(ResultStatus.Invalid, errorMessage, validationErrors);
}
