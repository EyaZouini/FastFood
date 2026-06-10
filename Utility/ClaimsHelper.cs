using System.Security.Claims;

namespace FastFood.Utility
{
    public static class ClaimsHelper
    {
        public static string GetUserId(ClaimsPrincipal user)
        {
            var identity = user.Identity as ClaimsIdentity;
            var claim = identity?.FindFirst(ClaimTypes.NameIdentifier);
            return claim?.Value ?? string.Empty;
        }
    }
}
