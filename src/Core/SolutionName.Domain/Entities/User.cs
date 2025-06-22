using Microsoft.AspNetCore.Identity;
using SolutionName.Persistence.Entities;

namespace SolutionName.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}


