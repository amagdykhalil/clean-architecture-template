using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Features.Auth.Commands.RevokeToken;

namespace SolutionName.Application.Tests.Features.Auth.Commands
{
    public class RevokeTokenCommandHandlerTests
    {
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly RevokeTokenCommandHandler _handler;
        private readonly DateTime _utcNow;

        public RevokeTokenCommandHandlerTests()
        {
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _utcNow = DateTime.UtcNow;
            _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(_utcNow);

            _handler = new RevokeTokenCommandHandler(
                _refreshTokenRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _dateTimeProviderMock.Object
            );
        }

        [Fact]
        public async Task Handle_EmptyToken_ReturnsNoContent()
        {
            // Arrange
            var command = new RevokeTokenCommand("");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _refreshTokenRepositoryMock.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NullToken_ReturnsNoContent()
        {
            // Arrange
            var command = new RevokeTokenCommand(null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _refreshTokenRepositoryMock.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidOrInactiveToken_ReturnsNoContent()
        {
            // Arrange
            var command = new RevokeTokenCommand("invalid-token");

            _refreshTokenRepositoryMock.Setup(x => x.GetAsync(command.Token, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshToken?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AlreadyRevokedToken_ReturnsNoContent()
        {
            // Arrange
            var command = new RevokeTokenCommand("revoked-token");
            var refreshToken = new RefreshToken
            {
                Token = command.Token,
                UserId = 1,
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                RevokedOn = DateTime.UtcNow.AddDays(-1)
            };

            _refreshTokenRepositoryMock.Setup(x => x.GetAsync(command.Token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidToken_RevokesTokenAndSavesChanges()
        {
            // Arrange
            var command = new RevokeTokenCommand("valid-token");
            var refreshToken = new RefreshToken
            {
                Token = command.Token,
                UserId = 1,
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                RevokedOn = null
            };

            _refreshTokenRepositoryMock.Setup(x => x.GetAsync(command.Token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(_utcNow, refreshToken.RevokedOn!.Value, TimeSpan.FromSeconds(1));

            _refreshTokenRepositoryMock.Verify(x => x.GetAsync(command.Token, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}