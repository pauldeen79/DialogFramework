namespace DialogFramework.Abstractions.Results;

public record Result<T> : Result where T: class
{
    private Result(T? value, string? errorMessage) : base(errorMessage) => Value = value;
    public T? Value { get; }
    public static Result<T> Success(T value) => new Result<T>(value, null);
    public static new Result<T> Error(string errorMessage) => new Result<T>(null, errorMessage);
}

public record Result
{
    protected Result(string? errorMessage) => ErrorMessage = errorMessage;

    public bool IsSuccessful() => ErrorMessage == null;
    public string? ErrorMessage { get; }

    public static Result Success() => new Result((string?)null);
    public static Result Error(string errorMessage) => new Result(errorMessage);
}
