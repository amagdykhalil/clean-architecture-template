namespace SolutionName.Domain.Abstract
{
    public abstract class CreationTrackableSoftDeleteEntity : CreationTrackableEntity, ISoftDeleteable
    {
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
    }
}
