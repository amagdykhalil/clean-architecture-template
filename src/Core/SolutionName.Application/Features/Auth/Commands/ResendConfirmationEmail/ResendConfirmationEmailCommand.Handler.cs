using SolutionName.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;

namespace SolutionName.Application.Features.Auth.Commands.ResendConfirmationEmail
{
    public class ResendConfirmationEmailCommandHandler : ICommandHandler<ResendConfirmationEmailCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserEmailService _userEmailService;
        private readonly IUserActionLinkBuilder _userActionLinkBuilder;

        public ResendConfirmationEmailCommandHandler(
            IIdentityService identityService,
            IUserEmailService userEmailService,
            IUserActionLinkBuilder userActionLinkBuilder,
            IConfiguration configuration)
        {
            _identityService = identityService;
            _userEmailService = userEmailService;
            _userActionLinkBuilder = userActionLinkBuilder;
        }

        public async Task<Result> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByEmailIncludePersonAsync(request.Email, cancellationToken);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Result.Success();
            }

            var confirmationLink = await _userActionLinkBuilder.BuildEmailConfirmationLinkAsync(user, cancellationToken);
            await _userEmailService.SendConfirmationLinkAsync(user, request.Email, confirmationLink);
            return Result.Success();
        }
    }
}