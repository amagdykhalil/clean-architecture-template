
using SolutionName.Application.Contracts.Persistence.Base;
using SolutionName.Domain.Interfaces;
using System.Linq.Expressions;

namespace SolutionName.Persistence.Repositories.Base
{
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : class, IEntity
    {
        private readonly AppDbContext _dbContext;
        private DbSet<Entity> Entities { get; set; }

        public GenericRepository(AppDbContext context)
        {
            _dbContext = context;
            Entities = _dbContext.Set<Entity>();
        }

        public async Task<Entity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await Entities.FindAsync(id, cancellationToken);
        }

        public async Task<List<Entity>> GetAllAsNoTracking(CancellationToken cancellationToken = default)
        {
            return await Entities.AsNoTracking().ToListAsync(cancellationToken);
        }

        public IQueryable<Entity> GetAllAsTracking()
        {
            return Entities.AsQueryable();
        }

        public async Task AddRangeAsync(ICollection<Entity> entities, CancellationToken cancellationToken = default)
        {
            await Entities.AddRangeAsync(entities, cancellationToken);
        }

        public async Task AddAsync(Entity entity, CancellationToken cancellationToken = default)
        {
            await Entities.AddAsync(entity, cancellationToken);
        }

        public async Task UpdateRangeAsync<TProperty>(Func<Entity, TProperty> propertyExpression, Func<Entity, TProperty> valueExpression, CancellationToken cancellationToken = default)
        {
            await Entities.ExecuteUpdateAsync(x => x.SetProperty(propertyExpression, valueExpression), cancellationToken);
        }

        public async Task DeleteRangeAsync(Expression<Func<Entity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            await Entities.Where(predicate).ExecuteDeleteAsync(cancellationToken);
        }

        public void Delete(Entity entity)
        {
            Entities.Remove(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await Entities.Where(e => e.Id == id).ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<bool> isExistsById(int id, CancellationToken cancellationToken = default)
        {
            return await Entities.AnyAsync(e => e.Id == id, cancellationToken);
        }
    }
}


