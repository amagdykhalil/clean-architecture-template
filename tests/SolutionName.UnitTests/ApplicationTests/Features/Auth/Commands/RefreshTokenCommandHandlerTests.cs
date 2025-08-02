using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Features.Auth.Commands.RefreshToken;
using SolutionName.Application.Features.Auth.Models;
using Microsoft.Extensions.Options;

namespace SolutionName.Application.Tests.Features.Auth.Commands
{
    public class RefreshTokenCommandHandlerTests
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<ITokenProvider> _tokenProviderMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly RefreshTokenCommandHandler _handler;
        private readonly DateTime _utcNow;

        public RefreshTokenCommandHandlerTests()
        {
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _identityServiceMock = new Mock<IIdentityService>();
            _tokenProviderMock = new Mock<ITokenProvider>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _utcNow = DateTime.UtcNow;
            _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(_utcNow);

            // Mock RefreshTokenSettings
            var refreshTokenSettingsMock = new Mock<IOptions<RefreshTokenSettings>>();
            refreshTokenSettingsMock.Setup(x => x.Value).Returns(new RefreshTokenSettings
            {
                ExpirationDays = 7
            });

            _handler = new RefreshTokenCommandHandler(
                _refreshTokenRepositoryMock.Object,
                _identityServiceMock.Object,
                _tokenProviderMock.Object,
                _unitOfWorkMock.Object,
                _dateTimeProviderMock.Object,
                refreshTokenSettingsMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRefreshToken_ReturnsNewAccessAndRefreshToken()
        {
            // Arrange
            var command = new RefreshTokenCommand("valid-refresh-token");
            var user = new User { Id = 1, Email = "test@example.com", DeletedAt = null };
            var existingRefreshToken = new RefreshToken
            {
                Token = command.Token,
                UserId = user.Id,
                User = user,
                ExpiresOn = _utcNow.AddDays(7),
                RevokedOn = null,
                CreatedOn = _utcNow.AddDays(-1) // Created 1 day ago
            };
            var newRefreshToken = new RefreshToken
            {
                Token = "new-refresh-token",
                UserId = user.Id,
                ExpiresOn = _utcNow.AddDays(7)
            };
            var accessToken = "new-access-token";
            var tokenExpiration = _utcNow.AddHours(1);

            _refreshTokenRepositoryMock.Setup(x => x.GetWithUserAsync(command.Token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingRefreshToken);

            _identityServiceMock.Setup(x => x.IsEmailConfirmedAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

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

            _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Handle_InvalidOrInactiveToken_ReturnsError()
        {
            // Arrange
            var command = new RefreshTokenCommand("invalid-token");

            _refreshTokenRepositoryMock.Setup(x => x.GetWithUserAsync(command.Token, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshToken?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);

            _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RevokedToken_ReturnsError()
        {
            // Arrange
            var command = new RefreshTokenCommand("revoked-token");
            var user = new User { Id = 1, Email = "test@example.com" };
            var revokedRefreshToken = new RefreshToken
            {
                Token = command.Token,
                UserId = user.Id,
                User = user,
                ExpiresOn = _utcNow.AddDays(7),
                RevokedOn = _utcNow.AddDays(-1), // Revoked 1 day ago
                CreatedOn = _utcNow.AddDays(-2)
            };

            _refreshTokenRepositoryMock.Setup(x => x.GetWithUserAsync(command.Token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(revokedRefreshToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);

            _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ExpiredToken_ReturnsError()
        {
            // Arrange
            var command = new RefreshTokenCommand("expired-token");
            var user = new User { Id = 1, Email = "test@example.com" };
            var expiredRefreshToken = new RefreshToken
            {
                Token = command.Token,
                UserId = user.Id,
                User = user,
                ExpiresOn = _utcNow.AddDays(7),
                RevokedOn = null,
                CreatedOn = _utcNow.AddDays(-10) // Created 10 days ago (expired based on 7-day setting)
            };

            _refreshTokenRepositoryMock.Setup(x => x.GetWithUserAsync(command.Token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expiredRefreshToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);

            _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EmailNotConfirmed_ReturnsError()
        {
            // Arrange
            var command = new RefreshTokenCommand("valid-token");
            var user = new User { Id = 1, Email = "test@example.com", DeletedAt = null };
            var refreshToken = new RefreshToken
            {
                Token = command.Token,
                UserId = user.Id,
                User = user,
                ExpiresOn = _utcNow.AddDays(7),
                RevokedOn = null,
                CreatedOn = _utcNow.AddDays(-1)
            };

            _refreshTokenRepositoryMock.Setup(x => x.GetWithUserAsync(command.Token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            _identityServiceMock.Setup(x => x.IsEmailConfirmedAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);

            _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UserDeleted_ReturnsError()
        {
            // Arrange
            var command = new RefreshTokenCommand("valid-token");
            var user = new User { Id = 1, Email = "test@example.com", DeletedAt = DateTime.UtcNow };
            var refreshToken = new RefreshToken
            {
                Token = command.Token,
                UserId = user.Id,
                User = user,
                ExpiresOn = _utcNow.AddDays(7),
                RevokedOn = null,
                CreatedOn = _utcNow.AddDays(-1)
            };

            _refreshTokenRepositoryMock.Setup(x => x.GetWithUserAsync(command.Token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            _identityServiceMock.Setup(x => x.IsEmailConfirmedAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);

            _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}