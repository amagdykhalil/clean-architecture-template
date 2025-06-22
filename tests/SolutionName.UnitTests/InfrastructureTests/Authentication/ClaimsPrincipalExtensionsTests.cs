using Infrastructure.Authentication;
using System.Security.Claims;


namespace SolutionName.Infrastructure.Tests.Authentication
{
    public class ClaimsPrincipalExtensionsTests
    {
        [Fact]
        public void GetUserId_WithValidUserIdClaim_ReturnsUserId()
        {
            // Arrange
            var userId = 123;
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act
            var result = principal.GetUserId();

            // Assert
            Assert.Equal(userId, result);
        }

        [Fact]
        public void GetUserId_WithInvalidUserIdClaim_ThrowsApplicationException()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "invalid-id")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act & Assert
            Assert.Throws<ApplicationException>(() => principal.GetUserId());
        }

        [Fact]
        public void GetUserId_WithNullPrincipal_ThrowsApplicationException()
        {
            // Arrange
            ClaimsPrincipal? principal = null;

            // Act & Assert
            Assert.Throws<ApplicationException>(() => principal.GetUserId());
        }

        [Fact]
        public void GetUserId_WithMissingUserIdClaim_ThrowsApplicationException()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act & Assert
            Assert.Throws<ApplicationException>(() => principal.GetUserId());
        }
    }
}