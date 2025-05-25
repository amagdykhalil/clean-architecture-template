namespace SolutionName.Domain.Interfaces
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        int CreatedBy { get; set; }
        DateTime? LastModifiedAt { get; set; }
        int? LastModifiedBy { get; set; }
    }
}


