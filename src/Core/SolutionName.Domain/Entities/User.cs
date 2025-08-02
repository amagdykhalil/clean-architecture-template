using Microsoft.AspNetCore.Identity;

namespace SolutionName.Domain.Entities
{
    public class User : IdentityUser<int>, ISoftDeleteable
    {
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
    }
}


