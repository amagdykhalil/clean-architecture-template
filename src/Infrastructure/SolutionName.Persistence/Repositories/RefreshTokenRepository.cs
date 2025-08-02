using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Contracts.Persistence;
using SolutionName.Application.Features.Auth.Models;
using SolutionName.Domain.Entities;
using SolutionName.Persistence;
using SolutionName.Persistence.Repositories.Base;
using System.Security.Cryptography;

namespace SolutionName.Persistence.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly AppDbContext _context;
        private readonly int _refreshTokenLifetime;
        private readonly IDateTimeProvider _dateTimeProvider;
        public RefreshTokenRepository(AppDbContext context, IOptions<RefreshTokenSettings> refreshTokenSettings, IDateTimeProvider dateTimeProvider) : base(context)
        {
            _context = context;
            _refreshTokenLifetime = refreshTokenSettings.Value.ExpirationDays;
            _dateTimeProvider = dateTimeProvider;
        }


        public async Task<RefreshToken?> GetActiveRefreshTokenAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(r => r.RevokedOn == null && r.ExpiresOn > _dateTimeProvider.UtcNow && r.UserId == userId, cancellationToken);
        }

        public RefreshToken GenerateRefreshToken(int userId)
        {
            var randomNumber = new byte[32];
            string Token;
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                Token = Convert.ToBase64String(randomNumber);
            }

            return new RefreshToken
            {
                Token = Token,
                CreatedOn = _dateTimeProvider.UtcNow,
                ExpiresOn = _dateTimeProvider.UtcNow.AddDays(_refreshTokenLifetime),
                UserId = userId
            };
        }

        public async Task<RefreshToken?> GetWithUserAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
        }

        public async Task<RefreshToken?> GetAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
        }
    }
}


