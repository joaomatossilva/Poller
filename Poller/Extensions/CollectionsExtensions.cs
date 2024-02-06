namespace Poller.Extensions;

using System.Linq.Expressions;
using Exceptions;
using MediatR;
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

    public static async Task<Result<List<T>>> ToListResultAsync<T>(this IQueryable<T> collection, CancellationToken cancellationToken = default)
    {
        var list = await collection.ToListAsync(cancellationToken);
        return Result<List<T>>.Success(list);
    }

    public static async Task<Result<Unit>> SaveChangesResultAsync(this DbContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<Unit>.Error(ex);
        }

        return Result<Unit>.Success(Unit.Value);
    }

    public static async Task<Result<T>> MapSaveChangesResultAsync<T>(this Task<Result<T>> result, DbContext dbContext, CancellationToken cancellationToken = default)
    {
        var previous = await result;
        return await dbContext.SaveChangesResultAsync(cancellationToken)
            .MapAsync((x, ct) => Task.FromResult(previous), cancellationToken);
    }
}