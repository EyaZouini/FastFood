using System.Security.Claims;
using FastFood.Utility;
using Xunit;

namespace FastFood.Tests.Utility
{
    public class ClaimsHelperTests
    {
        [Fact]
        public void GetUserId_ReturnsUserId_WhenClaimExists()
        {
            var userId = "user-123";
            var principal = MakePrincipal(userId);

            var result = ClaimsHelper.GetUserId(principal);

            Assert.Equal(userId, result);
        }

        [Fact]
        public void GetUserId_ReturnsEmptyString_WhenNoIdentifierClaim()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Email, "test@test.com")
            }));

            var result = ClaimsHelper.GetUserId(principal);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetUserId_ReturnsEmptyString_WhenIdentityIsNull()
        {
            var principal = new ClaimsPrincipal();

            var result = ClaimsHelper.GetUserId(principal);

            Assert.Equal(string.Empty, result);
        }

        private static ClaimsPrincipal MakePrincipal(string userId) =>
            new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));
    }
}
