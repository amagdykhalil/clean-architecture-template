using SolutionName.Domain.Interfaces;
using System.Linq.Expressions;

namespace SolutionName.Application.Contracts.Persistence.Base
{
    /// <summary>
    /// Generic repository interface that defines common database operations for entities.
    /// </summary>
    /// <typeparam name="Entity">The type of entity this repository handles.</typeparam>
    public interface IGenericRepository<Entity> where Entity : IEntity
    {
        Task<Entity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Entity>> GetAllAsNoTracking(CancellationToken cancellationToken = default);
        IQueryable<Entity> GetAllAsTracking();
        Task AddRangeAsync(ICollection<Entity> entities, CancellationToken cancellationToken = default);
        Task AddAsync(Entity entity, CancellationToken cancellationToken = default);
        Task UpdateRangeAsync<TProperty>(Func<Entity, TProperty> propertyExpression, Func<Entity, TProperty> valueExpression, CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(Expression<Func<Entity, bool>> predicate, CancellationToken cancellationToken = default);
        void Delete(Entity entity);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> isExistsById(int id, CancellationToken cancellationToken = default);
    }
}



