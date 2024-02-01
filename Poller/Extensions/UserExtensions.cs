namespace Poller.Extensions;

using System.Security.Claims;
using Exceptions;

public static class UserExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new NoUserIdException();
    }
}