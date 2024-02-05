namespace Poller.Extensions;

using System.Linq.Expressions;
using Exceptions;
using Microsoft.EntityFrameworkCore;

public static class CollectionsExtensions
{
    public static async Task<Result<T>> FirstOrFailResultAsync<T>(this IQueryable<T> collection, CancellationToken cancellationToken = default)
    {
        var item = await collection.FirstOrDefaultAsync(cancellationToken);
        return item is null ? Result<T>.Error(new NotFoundException()) : Result<T>.Success(item);
    }

    public static async Task<Result<T>> FirstOrFailResultAsync<T>(this IQueryable<T> collection, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var item = await collection.FirstOrDefaultAsync(predicate, cancellationToken);
        return item is null ? Result<T>.Error(new NotFoundException()) : Result<T>.Success(item);
    }
}