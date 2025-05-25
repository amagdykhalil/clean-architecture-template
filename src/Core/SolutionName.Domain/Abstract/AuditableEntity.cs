using SolutionName.Domain.Interfaces;

namespace SolutionName.Domain.Abstract
{
    public abstract class AuditableEntity : Entity, IAuditable
    {
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}


