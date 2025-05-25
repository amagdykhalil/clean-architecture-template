namespace SolutionName.Application.Features.Auth.Commands.RevokeToken
{
    public class RevokeTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork) : ICommandHandler<RevokeTokenCommand>
    {
        public async Task<ApiResponse<object>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return NoContent();
            }

            var refreshToken = await refreshTokenRepository.GetAsync(request.Token);

            if (refreshToken is null || !refreshToken.IsActive)
            {
                return NoContent();
            }

            // Revoke the token
            refreshToken.RevokedOn = DateTime.UtcNow;
            await unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}
