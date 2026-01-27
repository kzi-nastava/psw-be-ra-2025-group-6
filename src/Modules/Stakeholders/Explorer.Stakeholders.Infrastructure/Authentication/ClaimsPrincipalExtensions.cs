using Explorer.Stakeholders.Core.Domain;
using System.Security.Claims;

namespace Explorer.Stakeholders.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static long PersonId(this ClaimsPrincipal user)
    {
        if (TryPersonId(user, out var personId)) return personId;
        return 0;
    }

    public static UserRole Role(this ClaimsPrincipal user)
    {
        if (TryRole(user, out var role)) return role;
        return UserRole.Tourist;
    }

    public static bool TryPersonId(this ClaimsPrincipal user, out long personId)
    {
        personId = 0;
        var value = user.Claims.FirstOrDefault(i => i.Type == "personId")?.Value
            ?? user.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value
            ?? user.Claims.FirstOrDefault(i => i.Type == "id")?.Value;
        return long.TryParse(value, out personId) && personId != 0;
    }

    public static bool TryRole(this ClaimsPrincipal user, out UserRole role)
    {
        role = UserRole.Tourist;
        var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        return roleClaim != null && Enum.TryParse(roleClaim, ignoreCase: true, out role);
    }
}
