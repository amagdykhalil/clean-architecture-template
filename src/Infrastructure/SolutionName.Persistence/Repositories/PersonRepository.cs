namespace SolutionName.Persistence.Repositories
{
    public class PersonRepository : GenericRepository<Person>, IPersonRepository
    {
        private readonly AppDbContext _context;
        public PersonRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
