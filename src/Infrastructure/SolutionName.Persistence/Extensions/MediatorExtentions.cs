using SolutionName.Domain.Abstract;
using SolutionName.Domain.Interfaces;
using MediatR;

namespace SolutionName.Persistence.Extensions
{
    /// <summary>
    /// Extension methods for working with the MediatR mediator in the context of domain events.
    /// </summary>
    public static class MediatorExtentions
    {
        /// <summary>
        /// Dispatches all domain events from entities in the current DbContext.
        /// </summary>
        /// <param name="mediator">The mediator instance to use for publishing events.</param>
        /// <param name="ctx">The DbContext containing entities with domain events.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, AppDbContext ctx)
        {
            var domainEvents = ctx.ChangeTracker
                .Entries<Entity>()
                .Select(entry => entry.Entity)
                .Where(e => e.DomainEvents != null && e.DomainEvents.Any())
                .SelectMany(entity =>
                {
                    IReadOnlyCollection<IDomainEvent> domainEvents = entity.DomainEvents;

                    entity.ClearDomainEvents();

                    return domainEvents;
                })
                .ToList();

            foreach (IDomainEvent domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}


