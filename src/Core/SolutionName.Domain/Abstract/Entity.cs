using SolutionName.Domain.Interfaces;

namespace SolutionName.Domain.Abstract
{
    public abstract class Entity : IEntity
    {
        public int Id { get; set; }
    }
}


