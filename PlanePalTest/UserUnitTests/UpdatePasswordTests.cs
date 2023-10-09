using AutoMapper;
using FluentValidation.TestHelper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using PlanePal.DTOs.User;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Implementation;
using PlanePal.Services.Interfaces;
using PlanePal.Services.Shared;
using PlanePal.Validators;
using Xunit;

namespace PlanePal.UserUnitTests
{
    public class UpdatePasswordTests
    {
        // sut stands for system under test
        private readonly IUserService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IAzureStorage _azureStorage = Substitute.For<IAzureStorage>();
        private readonly UpdatePasswordValidator _validator;
        private readonly UpdatePasswordDTO _updatePasswordDto;

        public UpdatePasswordTests()
        {
            _sut = new UserService(_unitOfWork, _mapper, _azureStorage);
            _validator = new();
            _updatePasswordDto = new();
        }

        [Fact]
        public async Task ChangePassword_InputIncorrectId_ReturnsError()
        {
            // Arange
            var changePasswordDto = new UpdatePasswordDTO { NewPassword = "test", OldPassword = "test" };
            _unitOfWork.UserRepository.GetOne(Arg.Any<string>()).ReturnsNull();
            var email = "testemail@gmail.com";

            // Act
            var result = await _sut.UpdatePassword(changePasswordDto, email);

            // Assert
            Assert.Null(result.Data);
            Assert.Equal($"User with an id {email} not found", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task ChangePassword_IncorrectOldPassword_ReturnsError()
        {
            // Arange
            var changePasswordDto = new UpdatePasswordDTO { NewPassword = "test", OldPassword = "test" };
            var user = new User();
            user.Email = "testemail@gmail.com";
            user.Password = "oldPassword";
            user.Salt = "salt";
            _unitOfWork.UserRepository.GetOne(user.Email).Returns(user);

            // Act
            var result = await _sut.UpdatePassword(changePasswordDto, "testemail@gmail.com");

            // Assert
            Assert.Null(result.Data);
            Assert.Equal("Incorrect old password", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task ChangePassword_HappyPath_ReturnsSuccess()
        {
            // Arange
            var changePasswordDto = new UpdatePasswordDTO { OldPassword = "Testtest1#", NewPassword = "Testtest33#" };
            var user = new User();
            user.Email = "testemail@gmail.com";
            user.Salt = HashnSaltService.GenerateSalt();
            user.Password = HashnSaltService.HashWithSalt("Testtest1#", user.Salt);
            _unitOfWork.UserRepository.GetOne(user.Email).Returns(user);
            _unitOfWork.UserRepository.SaveChanges().Returns(1);

            // Act
            var result = await _sut.UpdatePassword(changePasswordDto, "testemail@gmail.com");

            // Assert
            Assert.Equal("Password successfully changed", result.Message);
            Assert.True(result.Success);
        }

        [Fact]
        public void ChangePassword_EmptyOldPassword_ReturnsError()
        {
            // Arrange
            _updatePasswordDto.OldPassword = "";

            // Act
            var result = _validator.TestValidate(_updatePasswordDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updatePassword => updatePassword.OldPassword)
                .WithErrorMessage("'Old Password' must not be empty.");
        }

        [Fact]
        public void ChangePassword_EmptyNewPassword_ReturnsError()
        {
            // Arrange
            _updatePasswordDto.NewPassword = "";

            // Act
            var result = _validator.TestValidate(_updatePasswordDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updatePassword => updatePassword.NewPassword)
                .WithErrorMessage("'New Password' must not be empty.");
        }

        [Fact]
        public void ChangePassword_InvalidNewPasswordMinimumLength_ReturnsError()
        {
            // Arrange
            _updatePasswordDto.NewPassword = "Test1#";

            // Act
            var result = _validator.TestValidate(_updatePasswordDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updatePassword => updatePassword.NewPassword);
        }

        [Fact]
        public void ChangePassword_NewPasswordNoUppercase_ReturnsError()
        {
            // Arrange
            _updatePasswordDto.NewPassword = "testtest1#";

            // Act
            var result = _validator.TestValidate(_updatePasswordDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updatePassword => updatePassword.NewPassword);
        }

        [Fact]
        public void ChangePassword_NewPasswordNoLowercase_ReturnsError()
        {
            // Arrange
            _updatePasswordDto.NewPassword = "TESTTEST1#";

            // Act
            var result = _validator.TestValidate(_updatePasswordDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updatePassword => updatePassword.NewPassword);
        }

        [Fact]
        public void ChangePassword_NewPasswordNoNumbers_ReturnsError()
        {
            // Arrange
            _updatePasswordDto.NewPassword = "TESTTESTT#";

            // Act
            var result = _validator.TestValidate(_updatePasswordDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updatePassword => updatePassword.NewPassword);
        }

        [Fact]
        public void ChangePassword_NewPasswordNoSpecialCharacters_ReturnsError()
        {
            // Arrange
            _updatePasswordDto.NewPassword = "TESTTESTTT";

            // Act
            var result = _validator.TestValidate(_updatePasswordDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updatePassword => updatePassword.NewPassword);
        }

        [Fact]
        public void ChangePassword_NewPassword_SuccessfulValidation_ReturnsSuccess()
        {
            // Arrange
            _updatePasswordDto.NewPassword = "Testtest1#";
            _updatePasswordDto.OldPassword = "Testtest1#";

            // Act
            var result = _validator.TestValidate(_updatePasswordDto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}