using SolutionName.Application.Abstractions.UserContext;
using SolutionName.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SolutionName.Persistence.Interceptors
{
    public sealed class UpdateAuditableInterceptor : SaveChangesInterceptor
    {
        private readonly IUserContext _userContext;
        public UpdateAuditableInterceptor(IUserContext userContext)
        {
            _userContext = userContext;
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is null)
            {
                return base.SavingChangesAsync(
                    eventData, result, cancellationToken);
            }

            UpdateAuditableEntities(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateAuditableEntities(DbContext context)
        {
            DateTime utcNow = DateTime.UtcNow;
            var entities = context.ChangeTracker.Entries<IAuditable>().ToList();

            foreach (EntityEntry<IAuditable> entry in entities)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.CreatedBy = _userContext.UserId;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedAt = utcNow;
                    entry.Entity.LastModifiedBy = _userContext.UserId;
                }
            }

        }
    }
}


