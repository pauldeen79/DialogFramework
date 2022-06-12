namespace DialogFramework.Abstractions.Results;

public record Result<T> : Result where T: class
{
    private Result(T? value, ResultStatus status, string? errorMessage, IEnumerable<ValidationError> validationErrors)
        : base(status, errorMessage, validationErrors) => Value = value;
    public T? Value { get; }
    public static Result<T> Success(T value) => new Result<T>(value, ResultStatus.Ok, null, Enumerable.Empty<ValidationError>());
    public static new Result<T> Error(ResultStatus status, string errorMessage) => new Result<T>(null, status, errorMessage, Enumerable.Empty<ValidationError>());
    public static new Result<T> Invalid(IEnumerable<ValidationError> validationErrors) => new Result<T>(null, ResultStatus.Invalid, null, validationErrors);
}

public record Result
{
    protected Result(ResultStatus status, string? errorMessage, IEnumerable<ValidationError> validationErrors)
    {
        Status = status;
        ErrorMessage = errorMessage;
        ValidationErrors = new ValueCollection<ValidationError>(validationErrors);
    }

    public bool IsSuccessful() => ErrorMessage == null;
    public string? ErrorMessage { get; }
    public ResultStatus Status { get; }
    public IReadOnlyCollection<ValidationError> ValidationErrors { get; }

    public static Result Success() => new Result(ResultStatus.Ok, null, Enumerable.Empty<ValidationError>());
    public static Result Error(ResultStatus status, string errorMessage) => new Result(status, errorMessage, Enumerable.Empty<ValidationError>());
    public static Result Invalid(IEnumerable<ValidationError> validationErrors) => new Result(ResultStatus.Invalid, null, validationErrors);
}
