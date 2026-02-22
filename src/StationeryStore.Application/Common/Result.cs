namespace StationeryStore.Application.Common;

/// <summary>
/// Result pattern for operation outcomes
/// </summary>
public class Result
{
    public bool IsSuccess { get; protected set; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; protected set; } = string.Empty;
    public List<string> Errors { get; protected set; } = new();
    
    protected Result() { }
    
    public static Result Success() => new() { IsSuccess = true };
    
    public static Result Failure(string error, params string[] errors)
        => new() { IsSuccess = false, Error = error, Errors = errors.ToList() };
    
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(string error, params string[] errors) => Result<T>.Failure(error, errors);
}

/// <summary>
/// Generic result with value
/// </summary>
public class Result<T> : Result
{
    public T? Value { get; protected set; }

    protected Result() { }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };

#pragma warning disable CS0108 // Intentionally hiding base class methods to return correct generic type
    public new static Result<T> Failure(string error, params string[] errors)
        => new() { IsSuccess = false, Error = error, Errors = errors.ToList() };
#pragma warning restore CS0108
}
