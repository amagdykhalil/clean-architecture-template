namespace SolutionName.Domain.Interfaces
{
    public interface IAuditable : ICreationTrackable
    {
        DateTime? LastModifiedAt { get; set; }
        int? LastModifiedBy { get; set; }
    }
}


