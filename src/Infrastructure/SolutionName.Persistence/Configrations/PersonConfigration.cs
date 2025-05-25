using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolutionName.Persistence.Entities;

namespace SolutionName.Persistence.Configrations
{
    public class PersonConfigration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {

        }
    }
}
