namespace SolutionName.Domain.Abstract
{
    public abstract class AuditableEntity : CreationTrackableEntity, IAuditable
    {
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}


