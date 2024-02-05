namespace Poller;

public readonly struct Result<T>
{
    private T? Value { get; }
    private bool IsSuccess { get; }
    private Exception? Exception { get; }

    public Result(T value)
    {
        Value = value;
        IsSuccess = true;
        Exception = null;
    }

    private Result(Exception exception)
    {
        IsSuccess = false;
        Exception = exception;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Error(Exception exception) => new(exception);

    public TOut Match<TOut>(Func<T, TOut> success, Func<Exception, TOut> error) =>
        IsSuccess ? success(Value) : error(Exception);

    public async Task<TOut> MatchAsync<TOut>(Func<T, Task<TOut>> success, Func<Exception, Task<TOut>> error) =>
        IsSuccess ? await success(Value) : await error(Exception);

    public Result<TOut> Map<TOut>(Func<T, Result<TOut>> action) =>
        IsSuccess ? action(Value) : Result<TOut>.Error(Exception);

    public async Task<Result<TOut>> MapAsync<TOut>(Func<T, CancellationToken, Task<Result<TOut>>> action, CancellationToken ct) =>
        IsSuccess ? await action(Value, ct) : Result<TOut>.Error(Exception);
}