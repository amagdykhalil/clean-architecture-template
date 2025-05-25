namespace SolutionName.Application.Features.People.Commands.Add
{
    public record AddPersonCommand(string FirstName, string LastName, DateTime DateOfBirth) : ICommand;
}
