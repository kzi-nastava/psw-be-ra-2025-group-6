using Explorer.Stakeholders.Core.Domain;
using System.Security.Claims;

namespace Explorer.Stakeholders.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static long PersonId(this ClaimsPrincipal user)
        => long.Parse(user.Claims.First(i => i.Type == "personId").Value);

    public static UserRole Role(this ClaimsPrincipal user)
    {
        var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        if (roleClaim == null)
            throw new Exception("Role claim not found");

        return Enum.Parse<UserRole>(roleClaim, ignoreCase: true);
    }
}
