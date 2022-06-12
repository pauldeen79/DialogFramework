namespace DialogFramework.Abstractions.Results;

public record Result<T> : Result where T: class
{
    private Result(T? value, ResultStatus status, string? errorMessage) : base(status, errorMessage) => Value = value;
    public T? Value { get; }
    public static Result<T> Success(T value) => new Result<T>(value, ResultStatus.Ok, null);
    public static new Result<T> Error(ResultStatus status, string errorMessage) => new Result<T>(null, status, errorMessage);
}

public record Result
{
    protected Result(ResultStatus status, string? errorMessage)
    {
        Status = status;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccessful() => ErrorMessage == null;
    public string? ErrorMessage { get; }
    public ResultStatus Status { get; }

    public static Result Success() => new Result(ResultStatus.Ok, null);
    public static Result Error(ResultStatus status, string errorMessage) => new Result(status, errorMessage);
}
