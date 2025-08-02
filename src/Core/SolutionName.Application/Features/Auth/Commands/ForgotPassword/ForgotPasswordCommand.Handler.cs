using SolutionName.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace SolutionName.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserEmailService _userEmailService;
        private readonly IUserActionLinkBuilder _userActionLinkBuilder;

        public ForgotPasswordCommandHandler(
            IIdentityService identityService,
            IUserEmailService userEmailService,
            IUserActionLinkBuilder userActionLinkBuilder)
        {
            _identityService = identityService;
            _userEmailService = userEmailService;
            _userActionLinkBuilder = userActionLinkBuilder;
        }

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByEmailIncludePersonAsync(request.Email, cancellationToken);

            if (user is not null && await _identityService.IsEmailConfirmedAsync(user, cancellationToken))
            {
                var link = await _userActionLinkBuilder.BuildPasswordResetLinkAsync(user, request.Email, cancellationToken);
                await _userEmailService.SendPasswordResetLinkAsync(user, request.Email, link);
            }

            // Don't reveal that the user does not exist or is not confirmed
            return Result.Success();
        }
    }
}