namespace Poller.Extensions;

public static class ResultExtensions
{
    public static async Task<Result<TOut>> MapAsync<TOut, T>(this Task<Result<T>> result,
        Func<T, CancellationToken, Task<Result<TOut>>> action, CancellationToken ct = default)
    {
        var res = await result;
        return await res.MapAsync(action, ct);
    }
}

//     public static async Task<TOut> MatchAsync<TOut, T>(this Task<Result<T>> result, Func<T, TOut> success, Func<Exception, TOut> error) =>
//         (await result).Match(success, error);
// }