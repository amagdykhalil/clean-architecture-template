namespace SolutionName.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Refresh token is required.")
                .MaximumLength(1000)
                .WithMessage("Refresh token must not exceed 1000 characters.");
        }
    }
}
