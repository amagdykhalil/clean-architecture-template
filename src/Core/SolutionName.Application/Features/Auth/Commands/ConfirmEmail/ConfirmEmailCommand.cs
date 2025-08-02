namespace SolutionName.Application.Features.Auth.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand : ICommand<string>
    {
        public int UserId { get; set; }
        public string Code { get; set; }
        public string? ChangedEmail { get; set; }
    }
}