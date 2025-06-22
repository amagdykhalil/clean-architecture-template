namespace SolutionName.Domain.Entities
{
    public class RefreshToken : Entity
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public bool IsActive => RevokedOn is null && !IsExpired;
    }
}


