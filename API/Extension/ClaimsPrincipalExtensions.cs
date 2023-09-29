using System.Security.Claims;

namespace API;

public static class ClaimsPrincipalExtensions
{
    public static string getUserName(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
