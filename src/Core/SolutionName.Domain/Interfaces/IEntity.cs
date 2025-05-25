namespace SolutionName.Domain.Interfaces
{
    internal interface IEntity
    {
        public int Id { get; set; }
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent eventItem);
        void RemoveDomainEvent(IDomainEvent eventItem);
        void ClearDomainEvents();
    }
}


