namespace Poller.Exceptions;

using Microsoft.AspNetCore.Mvc;

public static class ExceptionResultHandler
{
    public static IActionResult HandleIt(this Exception exception)
    {
        return exception switch
        {
            NotFoundException notFoundException => new NotFoundResult(),
            _ => throw exception
        };
    }
}