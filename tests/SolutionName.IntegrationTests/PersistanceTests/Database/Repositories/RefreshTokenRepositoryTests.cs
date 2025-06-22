

namespace SolutionName.IntegrationTests.Infrastructure.Database.Repositories
{
    [Collection(TestCollections.DatabaseTests)]
    public class RefreshTokenRepositoryTests
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;

        public RefreshTokenRepositoryTests(DatabaseTestEnvironmentFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
            _refreshTokenRepository = _serviceProvider.GetRequiredService<IRefreshTokenRepository>();
            _identityService = _serviceProvider.GetRequiredService<IIdentityService>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
        }

        [Fact]
        public async Task GenerateRefreshToken_ShouldCreateNewToken()
        {
            // Arrange
            var user = TestDataGenerators.UserFaker().Generate();
            var result = await _identityService.CreateUserAsync(user, "Test123!");

            // Act
            var token = _refreshTokenRepository.GenerateRefreshToken(user.Id);
            await _refreshTokenRepository.AddAsync(token);
            await _unitOfWork.SaveChangesAsync();

            // Assert
            Assert.NotNull(token);
            Assert.Equal(user.Id, token.UserId);
            Assert.NotNull(token.Token);
            Assert.True(token.ExpiresOn > DateTime.UtcNow); ///Time
            Assert.True(token.IsActive);
        }

        [Fact]
        public async Task GetActiveRefreshTokenAsync_ShouldReturnActiveToken()
        {
            // Arrange
            var user = TestDataGenerators.UserFaker().Generate();
            var result = await _identityService.CreateUserAsync(user, "Test123!");


            var token = _refreshTokenRepository.GenerateRefreshToken(user.Id);
            await _refreshTokenRepository.AddAsync(token);
            await _unitOfWork.SaveChangesAsync();

            // Act
            var activeToken = await _refreshTokenRepository.GetActiveRefreshTokenAsync(user.Id);

            // Assert
            Assert.NotNull(activeToken);
            Assert.Equal(token.Token, activeToken.Token);
            Assert.True(activeToken.IsActive);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnTokenByValue()
        {
            // Arrange
            var user = TestDataGenerators.UserFaker().Generate();
            var result = await _identityService.CreateUserAsync(user, "Test123!");


            var token = _refreshTokenRepository.GenerateRefreshToken(user.Id);
            await _refreshTokenRepository.AddAsync(token);
            await _unitOfWork.SaveChangesAsync();

            // Act
            var foundToken = await _refreshTokenRepository.GetAsync(token.Token);

            // Assert
            Assert.NotNull(foundToken);
            Assert.Equal(token.Token, foundToken.Token);
            Assert.Equal(user.Id, foundToken.UserId);
        }

        [Fact]
        public async Task GetWithUserAsync_ShouldReturnTokenWithUser()
        {
            // Arrange
            var user = TestDataGenerators.UserFaker().Generate();
            var result = await _identityService.CreateUserAsync(user, "Test123!");


            var token = _refreshTokenRepository.GenerateRefreshToken(user.Id);
            await _refreshTokenRepository.AddAsync(token);
            await _unitOfWork.SaveChangesAsync();

            // Act
            var foundToken = await _refreshTokenRepository.GetWithUserAsync(token.Token);

            // Assert
            Assert.NotNull(foundToken);
            Assert.NotNull(foundToken.User);
            Assert.Equal(user.Id, foundToken.User.Id);
        }
    }
}