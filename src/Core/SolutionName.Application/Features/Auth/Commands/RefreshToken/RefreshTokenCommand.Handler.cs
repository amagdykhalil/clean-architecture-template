

namespace SolutionName.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IIdentityService identityService,
        ITokenProvider tokenProvider,
        IUnitOfWork unitOfWork)
        : ICommandHandler<RefreshTokenCommand, AuthDTO>
    {
        public async Task<ApiResponse<AuthDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return ApiResponse<AuthDTO>.BadRequest("Invalid token");
            }

            var refreshToken = await refreshTokenRepository.GetWithUserAsync(request.Token);

            if (refreshToken is null || !refreshToken.IsActive)
            {
                return ApiResponse<AuthDTO>.BadRequest("Invalid token");
            }

            // Revoke old token
            refreshToken.RevokedOn = DateTime.UtcNow;

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

            return ApiResponse<AuthDTO>.Ok(authDto);
        }
    }
}
