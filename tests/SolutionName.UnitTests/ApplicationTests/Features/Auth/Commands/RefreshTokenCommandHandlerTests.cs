using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Features.Auth.Commands.RefreshToken;



namespace SolutionName.Application.Tests.Features.Auth.Commands
{
    public class RefreshTokenCommandHandlerTests : IClassFixture<LocalizationKeyFixture>
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<ITokenProvider> _tokenProviderMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IStringLocalizer<RefreshTokenCommandHandler>> _localizerMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly RefreshTokenCommandHandler _handler;
        private readonly DateTime _utcNow;

        public RefreshTokenCommandHandlerTests()
        {
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _identityServiceMock = new Mock<IIdentityService>();
            _tokenProviderMock = new Mock<ITokenProvider>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _localizerMock = new Mock<IStringLocalizer<RefreshTokenCommandHandler>>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _utcNow = DateTime.UtcNow;
            _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(_utcNow);

            // Setup default localization
            var localizedString = new LocalizedString(LocalizationKeys.Auth.InvalidToken, "Invalid or expired refresh token");
            _localizerMock.Setup(x => x[LocalizationKeys.Auth.InvalidToken])
                .Returns(localizedString);

            _handler = new RefreshTokenCommandHandler(
                _refreshTokenRepositoryMock.Object,
                _identityServiceMock.Object,
                _tokenProviderMock.Object,
                _unitOfWorkMock.Object,
                _localizerMock.Object,
                _dateTimeProviderMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRefreshToken_ReturnsNewAccessAndRefreshToken()
        {
            // Arrange
            var command = new RefreshTokenCommand("valid-refresh-token");
            var user = new User { Id = 1, Email = "test@example.com" };
            var existingRefreshToken = new Domain.Entities.RefreshToken
            {
                Token = command.Token,
                UserId = user.Id,
                User = user,
                ExpiresOn = _utcNow.AddDays(7),
                RevokedOn = null
            };
            var newRefreshToken = new Domain.Entities.RefreshToken
            {
                Token = "new-refresh-token",
                UserId = user.Id,
                ExpiresOn = _utcNow.AddDays(7)
            };
            var accessToken = "new-access-token";
            var tokenExpiration = _utcNow.AddHours(1);

            _refreshTokenRepositoryMock.Setup(x => x.GetWithUserAsync(command.Token))
                .ReturnsAsync(existingRefreshToken);

            _refreshTokenRepositoryMock.Setup(x => x.GenerateRefreshToken(user.Id))
                .Returns(newRefreshToken);

            _tokenProviderMock.Setup(x => x.Create(user))
                .ReturnsAsync(accessToken);

            _tokenProviderMock.Setup(x => x.GetAccessTokenExpiration())
                .Returns(tokenExpiration);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(accessToken, result.Value.AccessToken);
            Assert.Equal(user.Id, result.Value.UserId);
            Assert.Equal(tokenExpiration, result.Value.ExpiresOn);
            Assert.Equal(newRefreshToken.Token, result.Value.RefreshToken);
            Assert.Equal(newRefreshToken.ExpiresOn, result.Value.RefreshTokenExpiration);
            Assert.Equal(_utcNow, existingRefreshToken.RevokedOn!.Value, TimeSpan.FromSeconds(1));

            _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.RefreshToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyToken_ReturnsError()
        {
            // Arrange
            var command = new RefreshTokenCommand("");
            var errorMessage = "Invalid or expired refresh token";

            _localizerMock.Setup(x => x[LocalizationKeys.Auth.InvalidToken])
                .Returns(new LocalizedString(LocalizationKeys.Auth.InvalidToken, errorMessage));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.Errors.First());

            _refreshTokenRepositoryMock.Verify(x => x.GetWithUserAsync(It.IsAny<string>()), Times.Never);
            _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.RefreshToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidOrInactiveToken_ReturnsError()
        {
            // Arrange
            var command = new RefreshTokenCommand("invalid-token");
            var errorMessage = "Invalid or expired refresh token";

            _refreshTokenRepositoryMock.Setup(x => x.GetWithUserAsync(command.Token))
                .ReturnsAsync((Domain.Entities.RefreshToken?)null);

            _localizerMock.Setup(x => x[LocalizationKeys.Auth.InvalidToken])
                .Returns(new LocalizedString(LocalizationKeys.Auth.InvalidToken, errorMessage));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.Errors.First());

            _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.RefreshToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}