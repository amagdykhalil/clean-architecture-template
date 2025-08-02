using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Features.Auth.Models;
using Microsoft.Extensions.Options;

namespace SolutionName.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IIdentityService identityService,
        ITokenProvider tokenProvider,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IOptions<RefreshTokenSettings> refreshTokenSettings)
        : ICommandHandler<RefreshTokenCommand, AuthDTO>
    {
        public async Task<Result<AuthDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await refreshTokenRepository.GetWithUserAsync(request.Token, cancellationToken);

            if (refreshToken is null || !refreshToken.IsActive)
            {
                return Result<AuthDTO>.Unauthorized("Invalid or expired refresh token.");
            }

            // Check expiration using settings
            var expirationDays = refreshTokenSettings.Value.ExpirationDays;
            if (refreshToken.CreatedOn.AddDays(expirationDays) < dateTimeProvider.UtcNow)
            {
                return Result<AuthDTO>.Unauthorized("Invalid or expired refresh token.");
            }

            if (!await identityService.IsEmailConfirmedAsync(refreshToken.User, cancellationToken))
            {
                return Result<AuthDTO>.Unauthorized("You must confirm your email address before logging in.");
            }

            if (refreshToken.User.DeletedAt != null)
            {
                return Result<AuthDTO>.Unauthorized("Your account is inactive.");
            }

            // Revoke old token
            refreshToken.RevokedOn = dateTimeProvider.UtcNow;

            // Create and save new token
            var newRefreshToken = refreshTokenRepository.GenerateRefreshToken(refreshToken.UserId);
            await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

            var accessToken = await tokenProvider.Create(refreshToken.User);

            var authDto = new AuthDTO
            {
                AccessToken = accessToken,
                UserId = refreshToken.UserId,
                ExpiresOn = tokenProvider.GetAccessTokenExpiration(),
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn
            };

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AuthDTO>.Success(authDto);
        }
    }
}
