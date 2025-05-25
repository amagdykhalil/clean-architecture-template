using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SolutionName.Application.Abstractions.Infrastructure;
using SolutionName.Application.Abstractions.UserContext;
using SolutionName.Infrastructure.Authentication;
using SolutionName.Persistence.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Authentication;

/// <summary>
/// Implementation of ITokenProvider that generates and manages JWT tokens.
/// </summary>
internal sealed class TokenProvider : ITokenProvider
{
    private readonly JWTSettings _jwtSetting;
    private readonly IIdentityService _identityService;
    public TokenProvider(IOptions<JWTSettings> jwtSettingOptions, IIdentityService identityService)
    {
        _jwtSetting = jwtSettingOptions.Value;
        _identityService = identityService;
    }

    public async Task<string> Create(User user)
    {
        var roles = await _identityService.GetRolesAsync(user.Id);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            //new Claim(JwtRegisteredClaimNames.GivenName, user.Person.FirstName),
            //new Claim(JwtRegisteredClaimNames.FamilyName, user.Person.LastName),
            new(ClaimTypes.Email, user.Email)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSetting.Issuer,
            Audience = _jwtSetting.Audience,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSetting.AccessTokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Secret)), SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var SecurityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(SecurityToken);

        return accessToken;
    }

    /// <summary>
    /// Gets the expiration time for access tokens.
    /// </summary>
    /// <returns>The DateTime when the token will expire.</returns>
    public DateTime GetAccessTokenExpiration()
    {
        return DateTime.UtcNow.AddMinutes(_jwtSetting.AccessTokenExpirationMinutes);
    }
}


