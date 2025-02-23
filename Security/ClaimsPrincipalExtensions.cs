using System;
using System.Security.Claims;

namespace Umbrella_Server.Security
{
    public static class ClaimsPrincipalExtension
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User is not authenticated.");
            return Guid.Parse(userIdClaim);
        }
        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(emailClaim))
                throw new UnauthorizedAccessException("User email not found.");
            return emailClaim;
        }

    }
}