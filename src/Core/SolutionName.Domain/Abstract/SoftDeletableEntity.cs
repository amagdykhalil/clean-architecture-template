namespace SolutionName.Domain.Abstract
{
    public abstract class SoftDeletableEntity : Entity, ISoftDeleteable
    {
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
    }
}


