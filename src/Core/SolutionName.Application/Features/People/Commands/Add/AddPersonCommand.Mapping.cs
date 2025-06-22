using SolutionName.Persistence.Entities;

namespace SolutionName.Application.Features.People.Commands.Add
{
    /// <summary>
    /// Extension methods for mapping from and to Person entity.
    /// </summary>
    public static class AddPersonCommandMappingExtensions
    {
        //Mapping Template Example
        public static Person MapToPerson(this AddPersonCommand command)
            => new Person
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                BirthDate = command.DateOfBirth
            };
    }
}
