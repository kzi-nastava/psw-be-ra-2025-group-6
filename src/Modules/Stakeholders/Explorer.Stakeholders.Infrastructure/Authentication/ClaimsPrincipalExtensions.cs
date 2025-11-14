using System.Security.Claims;

namespace Explorer.Stakeholders.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static long PersonId(this ClaimsPrincipal user)
        => long.Parse(user.Claims.First(i => i.Type == "personId").Value);
}
