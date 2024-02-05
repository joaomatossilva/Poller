namespace Poller.Extensions;

using System.Linq.Expressions;
using System.Reflection;
using Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public static class ResultsExtensions
{
    public static async Task<TOut> Match<T,TOut>(this Task<Result<T>> resultTask, Func<T, TOut> success, Func<Exception, TOut> error) =>
        (await resultTask)
        .Match(success, error);

    public static IActionResult MatchPage<T>(this Result<T> resultTask, Action<T> success) =>
        resultTask.Match(item =>
        {
            success(item);
            return new PageResult();
        }, exception => exception.HandleIt());

    public static async Task<IActionResult> MatchPage<T>(this Task<Result<T>> resultTask, Action<T> success) =>
        (await resultTask)
        .MatchPage(success);

    public static async Task<IActionResult> MatchPageResult<T,TIn>(this Task<Result<T>> resultTask, Expression<Func<TIn, T>> success, TIn instance) =>
        (await resultTask)
        .MatchPage(item => { instance.AssignValue(item, success); });

    public static async Task<IActionResult> MatchPageResult<T>(this Task<Result<T>> resultTask) =>
        (await resultTask)
        .MatchPage(_ => { });

    public static async Task<IActionResult> MatchRedirectToPage<T>(this Task<Result<T>> resultTask, string page) =>
        (await resultTask)
        .Match(item => new RedirectToPageResult(page, item), exception => exception.HandleIt());

    private static void AssignValue<T, TIn>(this TIn instance, T value, Expression<Func<TIn, T>> expression)
    {
        var propertyInfo = (PropertyInfo)((MemberExpression)expression.Body).Member;
        propertyInfo.SetValue(instance, value, null);
    }
}