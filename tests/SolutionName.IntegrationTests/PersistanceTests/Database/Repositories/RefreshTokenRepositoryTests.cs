using SolutionName.IntegrationTests.PersistanceTests.Database.Common;

namespace SolutionName.IntegrationTests.PersistanceTests.Database.Repositories
{

    public class RefreshTokenRepositoryTests : BaseDatabaseTests
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IIdentityService _identityService;

        public RefreshTokenRepositoryTests(DatabaseTestEnvironmentFixture fixture) : base(fixture)
        {
            _refreshTokenRepository = ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
            _identityService = ServiceProvider.GetRequiredService<IIdentityService>();
        }


        [Fact]
        public async Task GenerateRefreshToken_CreatesNewToken()
        {
            // Arrange
            var user = TestDataGenerators.UserFaker().Generate();
            var result = await _identityService.CreateUserAsync(user);

            // Act
            var token = _refreshTokenRepository.GenerateRefreshToken(user.Id);
            await _refreshTokenRepository.AddAsync(token);
            await UnitOfWork.SaveChangesAsync();

            // Assert
            Assert.NotNull(token);
            Assert.Equal(user.Id, token.UserId);
            Assert.NotNull(token.Token);
            Assert.True(token.ExpiresOn > DateTime.UtcNow); ///Time
            Assert.True(token.IsActive);
        }

        [Fact]
        public async Task GetActiveRefreshTokenAsync_ReturnsActiveToken()
        {
            // Arrange
            var user = TestDataGenerators.UserFaker().Generate();
            var result = await _identityService.CreateUserAsync(user);


            var token = _refreshTokenRepository.GenerateRefreshToken(user.Id);
            await _refreshTokenRepository.AddAsync(token);
            await UnitOfWork.SaveChangesAsync();

            // Act
            var activeToken = await _refreshTokenRepository.GetActiveRefreshTokenAsync(user.Id);
            // Assert
            Assert.NotNull(activeToken);
            Assert.Equal(token.Token, activeToken.Token);
            Assert.True(activeToken.IsActive);
        }

        [Fact]
        public async Task GetAsync_ReturnsTokenByValue()
        {
            // Arrange
            var user = TestDataGenerators.UserFaker().Generate();
            var result = await _identityService.CreateUserAsync(user);


            var token = _refreshTokenRepository.GenerateRefreshToken(user.Id);
            await _refreshTokenRepository.AddAsync(token);
            await UnitOfWork.SaveChangesAsync();

            // Act
            var foundToken = await _refreshTokenRepository.GetAsync(token.Token);

            // Assert
            Assert.NotNull(foundToken);
            Assert.Equal(token.Token, foundToken.Token);
            Assert.Equal(user.Id, foundToken.UserId);
        }

        [Fact]
        public async Task GetWithUserAsync_ReturnsTokenWithUser()
        {
            // Arrange
            var user = TestDataGenerators.UserFaker().Generate();
            var result = await _identityService.CreateUserAsync(user);


            var token = _refreshTokenRepository.GenerateRefreshToken(user.Id);
            await _refreshTokenRepository.AddAsync(token);
            await UnitOfWork.SaveChangesAsync();

            // Act
            var foundToken = await _refreshTokenRepository.GetWithUserAsync(token.Token);

            // Assert
            Assert.NotNull(foundToken);
            Assert.NotNull(foundToken.User);
            Assert.Equal(user.Id, foundToken.User.Id);
        }

    }
}