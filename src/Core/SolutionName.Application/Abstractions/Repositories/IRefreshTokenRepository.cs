namespace SolutionName.Application.Contracts.Persistence
{
    /// <summary>
    /// Repository interface for managing refresh tokens.
    /// </summary>
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>, IRepository
    {
        Task<RefreshToken?> GetActiveRefreshTokenAsync(int UserId, CancellationToken cancellationToken = default);
        Task<RefreshToken?> GetAsync(string token, CancellationToken cancellationToken = default);
        Task<RefreshToken?> GetWithUserAsync(string token, CancellationToken cancellationToken = default);
        RefreshToken GenerateRefreshToken(int UserId);
    }
}


