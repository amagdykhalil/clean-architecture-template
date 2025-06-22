using Infrastructure.Authentication;
using Microsoft.Extensions.Options;
using SolutionName.Application.Abstractions.Services;
using SolutionName.Infrastructure.Authentication;
using SolutionName.Persistence.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SolutionName.Infrastructure.Tests.Authentication
{
    public class TokenProviderTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly JWTSettings _jwtSettings;
        private readonly TokenProvider _tokenProvider;
        private readonly DateTime _utcNow;

        public TokenProviderTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _utcNow = DateTime.UtcNow;
            _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(_utcNow);

            _jwtSettings = new JWTSettings
            {
                Secret = "your-256-bit-secret-your-256-bit-secret-your-256-bit-secret",
                Issuer = "test-issuer",
                Audience = "test-audience",
                AccessTokenExpirationMinutes = 60
            };

            var options = Options.Create(_jwtSettings);
            _tokenProvider = new TokenProvider(options, _identityServiceMock.Object, _dateTimeProviderMock.Object);
        }

        [Fact]
        public async Task Create_WithValidUser_ReturnsValidJwtToken()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Person = new Person
                {
                    FirstName = "Test",
                    LastName = "User"
                }
            };

            var roles = new[] { "User", "Admin" };

            _identityServiceMock.Setup(x => x.GetRolesAsync(user.Id))
                .ReturnsAsync(roles);

            // Act
            var token = await _tokenProvider.Create(user);

            // Assert
            Assert.NotNull(token);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Equal(_jwtSettings.Issuer, jwtToken.Issuer);
            Assert.Equal(_jwtSettings.Audience, jwtToken.Audiences.First());
            Assert.Equal(_utcNow, jwtToken.IssuedAt, TimeSpan.FromSeconds(1));
            Assert.Equal(_utcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes), jwtToken.ValidTo, TimeSpan.FromSeconds(1));

            // NameIdentifier: support both URI and "nameid"
            var nameIdClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.NameId);
            Assert.NotNull(nameIdClaim);
            Assert.Equal(user.Id.ToString(), nameIdClaim.Value);

            // GivenName
            var givenNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName);
            Assert.NotNull(givenNameClaim);
            Assert.Equal(user.Person.FirstName, givenNameClaim.Value);

            // FamilyName
            var familyNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.FamilyName);
            Assert.NotNull(familyNameClaim);
            Assert.Equal(user.Person.LastName, familyNameClaim.Value);

            // Email: support both URI and "email"
            var emailClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Email || c.Type == JwtRegisteredClaimNames.Email);
            Assert.NotNull(emailClaim);
            Assert.Equal(user.Email, emailClaim.Value);

            // Roles
            var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToArray();
            Assert.Equal(roles, roleClaims);
        }

        [Fact]
        public void GetAccessTokenExpiration_ReturnsCorrectExpirationTime()
        {
            // Act
            var expiration = _tokenProvider.GetAccessTokenExpiration();

            // Assert
            var expectedExpiration = _utcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
            Assert.Equal(expectedExpiration, expiration, TimeSpan.FromSeconds(1));
        }
    }
}