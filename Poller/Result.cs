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

    public Result<TOut> Map<TOut>(Func<T, Result<TOut>> action) =>
        IsSuccess ? action(Value) : Result<TOut>.Error(Exception);
}