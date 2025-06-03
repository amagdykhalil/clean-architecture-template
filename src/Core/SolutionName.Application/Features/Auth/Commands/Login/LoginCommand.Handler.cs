using Microsoft.Extensions.Logging;

namespace SolutionName.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler(
        IIdentityService identityService,
        ITokenProvider tokenProvider,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        ILogger<LoginCommandHandler> logger)
        : ICommandHandler<LoginCommand, AuthDTO>
    {
        public async Task<Result<AuthDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {

            var user = await identityService.GetUserAsync(request.Email, request.PasswordHash);

            if (user == null)
            {
                return Result<AuthDTO>.Error("Email or password is incorrect!");
            }

            var accessToken = await tokenProvider.Create(user);
            AuthDTO AuthInfo = new AuthDTO
            {
                AccessToken = accessToken,
                UserId = user.Id,
                ExpiresOn = tokenProvider.GetAccessTokenExpiration(),
            };


            var ActiveRefreshToken = await refreshTokenRepository.GetActiveRefreshTokenAsync(user.Id);

            if (ActiveRefreshToken != null)
            {
                AuthInfo.RefreshToken = ActiveRefreshToken.Token;
                AuthInfo.RefreshTokenExpiration = ActiveRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = refreshTokenRepository.GenerateRefreshToken(user.Id);

                await refreshTokenRepository.AddAsync(refreshToken);

                AuthInfo.RefreshToken = refreshToken.Token;
                AuthInfo.RefreshTokenExpiration = refreshToken.ExpiresOn;
            }

            return Result<AuthDTO>.Success(AuthInfo);
        }
    }
}
