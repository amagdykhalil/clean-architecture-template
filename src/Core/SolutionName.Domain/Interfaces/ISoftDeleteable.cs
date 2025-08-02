namespace SolutionName.Domain.Interfaces
{
    public interface ISoftDeleteable
    {
        public bool IsDeleted => DeletedAt.HasValue;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
    }
}


