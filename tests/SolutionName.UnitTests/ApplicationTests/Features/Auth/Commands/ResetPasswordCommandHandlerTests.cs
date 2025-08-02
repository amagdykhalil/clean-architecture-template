using SolutionName.Application.Abstractions.Services;
using SolutionName.Application.Features.Auth.Commands.ResetPassword;
using SolutionName.Tests.Common.DataGenerators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace SolutionName.Application.Tests.Features.Auth.Commands
{
    public class ResetPasswordCommandHandlerTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IUserEmailService> _userEmailServiceMock;
        private readonly Mock<IUserActionLinkBuilder> _userActionLinkBuilderMock;
        private readonly ResetPasswordCommandHandler _handler;
        private readonly User _user;

        public ResetPasswordCommandHandlerTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _userEmailServiceMock = new Mock<IUserEmailService>();
            _userActionLinkBuilderMock = new Mock<IUserActionLinkBuilder>();

            _handler = new ResetPasswordCommandHandler(
                _identityServiceMock.Object,
                _userEmailServiceMock.Object,
                _userActionLinkBuilderMock.Object
            );

            _user = TestDataGenerators.UserFaker().Generate();
        }

        [Fact]
        public async Task Handle_ValidResetCode_ReturnsSuccess()
        {
            // Arrange
            var code = "reset-code";
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var command = new ResetPasswordCommand(_user.Email, encodedCode, "NewPassword123!");

            _identityServiceMock.Setup(x => x.FindByEmailIncludePersonAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);
            _identityServiceMock.Setup(x => x.IsEmailConfirmedAsync(_user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _identityServiceMock.Setup(x => x.ResetPasswordAsync(_user, code, command.NewPassword, It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Success);

            _userActionLinkBuilderMock.Setup(x => x.BuildPasswordResetLinkAsync(_user, _user.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync("https://example.com/reset-password");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _userEmailServiceMock.Verify(x => x.SendPasswordChangedNotificationAsync(_user, _user.Email, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsSuccess()
        {
            // Arrange
            var command = new ResetPasswordCommand("notfound@example.com", "code", "NewPassword123!");

            _identityServiceMock.Setup(x => x.FindByEmailIncludePersonAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _userEmailServiceMock.Verify(x => x.SendPasswordChangedNotificationAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UserExistsButEmailNotConfirmed_ReturnsSuccess()
        {
            // Arrange
            var command = new ResetPasswordCommand(_user.Email, "code", "NewPassword123!");

            _identityServiceMock.Setup(x => x.FindByEmailIncludePersonAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);
            _identityServiceMock.Setup(x => x.IsEmailConfirmedAsync(_user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _userEmailServiceMock.Verify(x => x.SendPasswordChangedNotificationAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidCodeFormat_ReturnsSuccess()
        {
            // Arrange
            var command = new ResetPasswordCommand(_user.Email, "invalid-format", "NewPassword123!");

            _identityServiceMock.Setup(x => x.FindByEmailIncludePersonAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);
            _identityServiceMock.Setup(x => x.IsEmailConfirmedAsync(_user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _userEmailServiceMock.Verify(x => x.SendPasswordChangedNotificationAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ResetFails_ReturnsSuccess()
        {
            // Arrange
            var code = "reset-code";
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var command = new ResetPasswordCommand(_user.Email, encodedCode, "NewPassword123!");
            var failedResult = IdentityResult.Failed(new IdentityError { Description = "Invalid code" });

            _identityServiceMock.Setup(x => x.FindByEmailIncludePersonAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);
            _identityServiceMock.Setup(x => x.IsEmailConfirmedAsync(_user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _identityServiceMock.Setup(x => x.ResetPasswordAsync(_user, code, command.NewPassword, It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _userEmailServiceMock.Verify(x => x.SendPasswordChangedNotificationAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyResetCode_ReturnsSuccess()
        {
            // Arrange
            var command = new ResetPasswordCommand(_user.Email, "", "NewPassword123!");
            var failedResult = IdentityResult.Failed(new IdentityError { Description = "Invalid code" });

            _identityServiceMock.Setup(x => x.FindByEmailIncludePersonAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);
            _identityServiceMock.Setup(x => x.IsEmailConfirmedAsync(_user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _identityServiceMock.Setup(x => x.ResetPasswordAsync(_user, command.ResetCode, command.NewPassword, It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedResult);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _userEmailServiceMock.Verify(x => x.SendPasswordChangedNotificationAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [Fact]
        public async Task Handle_ValidResetCode_SendsNotificationEmail()
        {
            // Arrange
            var code = "reset-code";
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var command = new ResetPasswordCommand(_user.Email, encodedCode, "NewPassword123!");
            var resetLink = "https://example.com/reset-password";

            _identityServiceMock.Setup(x => x.FindByEmailIncludePersonAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);
            _identityServiceMock.Setup(x => x.IsEmailConfirmedAsync(_user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _identityServiceMock.Setup(x => x.ResetPasswordAsync(_user, code, command.NewPassword, It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Success);

            _userActionLinkBuilderMock.Setup(x => x.BuildPasswordResetLinkAsync(_user, _user.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(resetLink);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _userActionLinkBuilderMock.Verify(x => x.BuildPasswordResetLinkAsync(_user, _user.Email, It.IsAny<CancellationToken>()), Times.Once);
            _userEmailServiceMock.Verify(x => x.SendPasswordChangedNotificationAsync(_user, _user.Email, resetLink), Times.Once);
        }


    }
}