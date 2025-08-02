using SolutionName.Application.Abstractions.Services;

namespace SolutionName.Application.Features.Auth.Commands.RevokeToken
{
    public class RevokeTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
        : ICommandHandler<RevokeTokenCommand>
    {
        public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return Result.NoContent();
            }

            var refreshToken = await refreshTokenRepository.GetAsync(request.Token, cancellationToken);

            if (refreshToken is null || !refreshToken.IsActive)
            {
                return Result.NoContent();
            }

            // Revoke the token
            refreshToken.RevokedOn = dateTimeProvider.UtcNow;
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.NoContent();
        }
    }
}

