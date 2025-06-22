using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Features.Auth;
using SolutionName.Application.Features.Auth.Commands.RefreshToken;

namespace SolutionName.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IIdentityService identityService,
        ITokenProvider tokenProvider,
        IUnitOfWork unitOfWork,
        IStringLocalizer<RefreshTokenCommandHandler> localizer,
        IDateTimeProvider dateTimeProvider)
        : ICommandHandler<RefreshTokenCommand, AuthDTO>
    {
        public async Task<Result<AuthDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return Result<AuthDTO>.Error(localizer[LocalizationKeys.Auth.InvalidToken]);
            }

            var refreshToken = await refreshTokenRepository.GetWithUserAsync(request.Token);

            if (refreshToken is null || !refreshToken.IsActive)
            {
                return Result<AuthDTO>.Error(localizer[LocalizationKeys.Auth.InvalidToken]);
            }

            // Revoke old token
            refreshToken.RevokedOn = dateTimeProvider.UtcNow;

            // Create and save new token
            var newRefreshToken = refreshTokenRepository.GenerateRefreshToken(refreshToken.UserId);
            await refreshTokenRepository.AddAsync(newRefreshToken);

            var accessToken = await tokenProvider.Create(refreshToken.User);

            var authDto = new AuthDTO
            {
                AccessToken = accessToken,
                UserId = refreshToken.UserId,
                ExpiresOn = tokenProvider.GetAccessTokenExpiration(),
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn
            };

            await unitOfWork.SaveChangesAsync();

            return Result<AuthDTO>.Success(authDto);
        }
    }
}

