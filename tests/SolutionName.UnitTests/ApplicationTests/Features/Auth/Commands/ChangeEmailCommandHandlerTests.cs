using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Features.Auth.Commands.ChangeEmail;
using SolutionName.Tests.Common.DataGenerators;

namespace SolutionName.Application.Tests.Features.Auth.Commands
{
    public class ChangeEmailCommandHandlerTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IUserEmailService> _userEmailServiceMock;
        private readonly Mock<IUserActionLinkBuilder> _userActionLinkBuilderMock;
        private readonly ChangeEmailCommandHandler _handler;
        private readonly User _user;

        public ChangeEmailCommandHandlerTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _userEmailServiceMock = new Mock<IUserEmailService>();
            _userActionLinkBuilderMock = new Mock<IUserActionLinkBuilder>();

            _handler = new ChangeEmailCommandHandler(
                _identityServiceMock.Object,
                _userEmailServiceMock.Object,
                _userActionLinkBuilderMock.Object
            );

            _user = TestDataGenerators.UserFaker().Generate();
        }

        [Fact]
        public async Task Handle_ValidChangeEmail_ReturnsSuccess()
        {
            // Arrange
            var newEmail = "newemail@example.com";
            var command = new ChangeEmailCommand(_user.Id, newEmail);
            var changeEmailLink = "https://example.com/change-email";

            _identityServiceMock.Setup(x => x.IsExitsByEmail(newEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync((int?)null);

            _identityServiceMock.Setup(x => x.GetUserByIdIncludePersonAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _userActionLinkBuilderMock.Setup(x => x.BuildChangeEmailConfirmationLinkAsync(_user, newEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(changeEmailLink);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _userEmailServiceMock.Verify(x => x.SendConfirmationLinkAsync(_user, newEmail, changeEmailLink), Times.Once);
        }

        [Fact]
        public async Task Handle_EmailAlreadyExists_ReturnsSuccess()
        {
            // Arrange
            var newEmail = "existing@example.com";
            var command = new ChangeEmailCommand(_user.Id, newEmail);
            var existingUser = new User { Id = 2, Email = newEmail };

            _identityServiceMock.Setup(x => x.IsExitsByEmail(newEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _identityServiceMock.Verify(x => x.GetUserByIdIncludePersonAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _userActionLinkBuilderMock.Verify(x => x.BuildChangeEmailConfirmationLinkAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _userEmailServiceMock.Verify(x => x.SendConfirmationLinkAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsSuccess()
        {
            // Arrange
            var newEmail = "newemail@example.com";
            var command = new ChangeEmailCommand(999, newEmail);

            _identityServiceMock.Setup(x => x.IsExitsByEmail(newEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync((int?)null);

            _identityServiceMock.Setup(x => x.GetUserByIdIncludePersonAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _userActionLinkBuilderMock.Verify(x => x.BuildChangeEmailConfirmationLinkAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _userEmailServiceMock.Verify(x => x.SendConfirmationLinkAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EmailAlreadyExistsForSameUser_ReturnsSuccess()
        {
            // Arrange
            var newEmail = "newemail@example.com";
            var command = new ChangeEmailCommand(_user.Id, newEmail);

            _identityServiceMock.Setup(x => x.IsExitsByEmail(newEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _identityServiceMock.Verify(x => x.GetUserByIdIncludePersonAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _userActionLinkBuilderMock.Verify(x => x.BuildChangeEmailConfirmationLinkAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _userEmailServiceMock.Verify(x => x.SendConfirmationLinkAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidChangeEmail_SendsConfirmationEmail()
        {
            // Arrange
            var newEmail = "newemail@example.com";
            var command = new ChangeEmailCommand(_user.Id, newEmail);
            var changeEmailLink = "https://example.com/change-email";

            _identityServiceMock.Setup(x => x.IsExitsByEmail(newEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync((int?)null);

            _identityServiceMock.Setup(x => x.GetUserByIdIncludePersonAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _userActionLinkBuilderMock.Setup(x => x.BuildChangeEmailConfirmationLinkAsync(_user, newEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(changeEmailLink);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _userActionLinkBuilderMock.Verify(x => x.BuildChangeEmailConfirmationLinkAsync(_user, newEmail, It.IsAny<CancellationToken>()), Times.Once);
            _userEmailServiceMock.Verify(x => x.SendConfirmationLinkAsync(_user, newEmail, changeEmailLink), Times.Once);
        }

    }
}