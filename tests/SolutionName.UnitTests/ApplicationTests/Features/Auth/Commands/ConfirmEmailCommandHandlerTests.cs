using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Features.Auth.Commands.ConfirmEmail;
using SolutionName.Tests.Common.DataGenerators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace SolutionName.Application.Tests.Features.Auth.Commands
{
    public class ConfirmEmailCommandHandlerTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IUserEmailService> _userEmailServiceMock;
        private readonly Mock<IUserActionLinkBuilder> _userActionLinkBuilderMock;
        private readonly ConfirmEmailCommandHandler _handler;
        private readonly User _user;

        public ConfirmEmailCommandHandlerTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _userEmailServiceMock = new Mock<IUserEmailService>();
            _userActionLinkBuilderMock = new Mock<IUserActionLinkBuilder>();

            _handler = new ConfirmEmailCommandHandler(
                _identityServiceMock.Object,
                _userEmailServiceMock.Object,
                _userActionLinkBuilderMock.Object
            );

            _user = TestDataGenerators.UserFaker().Generate();
        }

        [Fact]
        public async Task Handle_ValidConfirmationCode_ReturnsSuccess()
        {
            // Arrange
            var code = "valid-code";
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var command = new ConfirmEmailCommand { UserId = _user.Id, Code = encodedCode };

            _identityServiceMock.Setup(x => x.GetUserByIdIncludePersonAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _identityServiceMock.Setup(x => x.ConfirmEmailAsync(_user, code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsSuccessWithNull()
        {
            // Arrange
            var code = "valid-code";
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var command = new ConfirmEmailCommand { UserId = 999, Code = encodedCode };

            _identityServiceMock.Setup(x => x.GetUserByIdIncludePersonAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Handle_InvalidCodeFormat_ReturnsSuccessWithNull()
        {
            // Arrange
            var command = new ConfirmEmailCommand { UserId = _user.Id, Code = "invalid-format" };

            _identityServiceMock.Setup(x => x.GetUserByIdIncludePersonAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Handle_ConfirmationFails_ReturnsSuccessWithNull()
        {
            // Arrange
            var code = "valid-code";
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var command = new ConfirmEmailCommand { UserId = _user.Id, Code = encodedCode };
            var failedResult = IdentityResult.Failed(new IdentityError { Description = "Invalid code" });

            _identityServiceMock.Setup(x => x.GetUserByIdIncludePersonAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _identityServiceMock.Setup(x => x.ConfirmEmailAsync(_user, code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Handle_UserWithoutPassword_ReturnsCreatePasswordLink()
        {
            // Arrange
            var code = "valid-code";
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var command = new ConfirmEmailCommand { UserId = _user.Id, Code = encodedCode };
            var createPasswordUrl = "https://example.com/create-password";

            _user.PasswordHash = null; // User has no password

            _identityServiceMock.Setup(x => x.GetUserByIdIncludePersonAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _identityServiceMock.Setup(x => x.ConfirmEmailAsync(_user, code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Success);

            _userActionLinkBuilderMock.Setup(x => x.BuildPasswordResetLinkAsync(_user, _user.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createPasswordUrl);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(createPasswordUrl, result.Value);

            _userEmailServiceMock.Verify(x => x.SendCreatePasswordLinkAsync(_user, _user.Email, createPasswordUrl), Times.Once);
        }

        [Fact]
        public async Task Handle_ChangeEmailConfirmation_ReturnsSuccess()
        {
            // Arrange
            var code = "valid-code";
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var newEmail = "newemail@example.com";
            var command = new ConfirmEmailCommand { UserId = _user.Id, Code = encodedCode, ChangedEmail = newEmail };

            _identityServiceMock.Setup(x => x.GetUserByIdIncludePersonAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _identityServiceMock.Setup(x => x.ChangeEmailAsync(_user, newEmail, code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Success);

            _identityServiceMock.Setup(x => x.SetUserNameAsync(_user, newEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);

            _userEmailServiceMock.Verify(x => x.SendEmailChangedNotificationAsync(_user, _user.Email, newEmail), Times.Once);
        }


        [Fact]
        public async Task Handle_ChangeEmailWithUserWithoutPassword_ReturnsCreatePasswordLink()
        {
            // Arrange
            var code = "valid-code";
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var newEmail = "newemail@example.com";
            var command = new ConfirmEmailCommand { UserId = _user.Id, Code = encodedCode, ChangedEmail = newEmail };
            var createPasswordUrl = "https://example.com/create-password";

            _user.PasswordHash = null; // User has no password

            _identityServiceMock.Setup(x => x.GetUserByIdIncludePersonAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _identityServiceMock.Setup(x => x.ChangeEmailAsync(_user, newEmail, code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Success);

            _identityServiceMock.Setup(x => x.SetUserNameAsync(_user, newEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Success);

            _userActionLinkBuilderMock.Setup(x => x.BuildPasswordResetLinkAsync(_user, _user.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createPasswordUrl);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(createPasswordUrl, result.Value);

            _userEmailServiceMock.Verify(x => x.SendEmailChangedNotificationAsync(_user, _user.Email, newEmail), Times.Once);
            _userEmailServiceMock.Verify(x => x.SendCreatePasswordLinkAsync(_user, _user.Email, createPasswordUrl), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyChangedEmail_ReturnsSuccess()
        {
            // Arrange
            var code = "valid-code";
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var command = new ConfirmEmailCommand { UserId = _user.Id, Code = encodedCode, ChangedEmail = "" };

            _identityServiceMock.Setup(x => x.GetUserByIdIncludePersonAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _identityServiceMock.Setup(x => x.ConfirmEmailAsync(_user, code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);

            _identityServiceMock.Verify(x => x.ChangeEmailAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _userEmailServiceMock.Verify(x => x.SendEmailChangedNotificationAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


    }
}