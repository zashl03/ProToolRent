
namespace ProToolRent.Application.Common;

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Conflict,
    Failure
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ErrorType ErrorType { get; }
    
    private Result(bool isSuccess, T? value, string? error, ErrorType errorType)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ErrorType = errorType;
    }

    public static Result<T> Success(T value) =>
        new(true, value, null, ErrorType.None);

    public static Result<T> Failure(string error, ErrorType type = ErrorType.Failure) =>
        new(false, default, error, type);

    public static Result<T> NotFound(string error = "Resource not found") =>
        new(false, default, error, ErrorType.NotFound);

    public static Result<T> Conflict(string error) =>
        new(false, default, error, ErrorType.Conflict);
}
