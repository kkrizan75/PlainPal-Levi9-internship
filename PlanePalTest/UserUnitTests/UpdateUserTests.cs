using AutoMapper;
using FluentValidation.TestHelper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using PlanePal.DTOs.User;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Implementation;
using PlanePal.Services.Interfaces;
using PlanePal.Validators;
using Xunit;

namespace PlanePal.UserUnitTests
{
    public class UpdateUserTests
    {
        private readonly IUserService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IAzureStorage _azureStorage = Substitute.For<IAzureStorage>();
        private readonly UpdateUserValidator _validator;
        private readonly UpdateUserDTO _updateUserDto;

        public UpdateUserTests()
        {
            _sut = new UserService(_unitOfWork, _mapper, _azureStorage);
            _validator = new();
            _updateUserDto = new();
        }

        [Fact]
        public async Task Update_InputIncorrectId_ReturnsError()
        {
            // Arange
            var email = "testemail@gmail.com";
            _unitOfWork.UserRepository.GetOne(Arg.Any<string>()).ReturnsNull();

            // Act
            var result = await _sut.Update(_updateUserDto, email);

            // Assert
            Assert.Null(result.Data);
            Assert.Equal($"User with an email {email} not found", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task Update_InvalidUpdate_ReturnsError()
        {
            // Arange
            _updateUserDto.FirstName = "Test";
            _updateUserDto.LastName = "Test";
            _updateUserDto.PhoneNumber = "+1234";
            _updateUserDto.DateOfBirth = DateTime.Now.AddYears(-23);
            _updateUserDto.Country = "Test";
            _updateUserDto.Street = "Test";
            _updateUserDto.City = "Test";
            _updateUserDto.StreetNumber = "Test1234";

            var user = new User();
            user.Email = "testemail@gmail.com";
            user.AddressId = 1;
            user.DocumentId = 1;
            user.Address = new Address
            {
                Id = 1
            };
            user.IdentificationDocument = new IdentificationDocument
            {
                Id = 1
            };
            _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(user.Email).Returns(user);
            _mapper.Map<User>(_updateUserDto).Returns(user);
            _unitOfWork.UserRepository.UpdateAndSave(user).Returns(0);

            // Act
            var validationResult = _validator.TestValidate(_updateUserDto);
            var result = await _sut.Update(_updateUserDto, "testemail@gmail.com");

            // Assert
            validationResult.ShouldNotHaveAnyValidationErrors();
            Assert.Equal("User not updated", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task Update_HappyPath_ReturnsSuccess()
        {
            // Arange
            _updateUserDto.FirstName = "Test";
            _updateUserDto.LastName = "Test";
            _updateUserDto.PhoneNumber = "+1234";
            _updateUserDto.DateOfBirth = DateTime.Now.AddYears(-23);
            _updateUserDto.Country = "Test";
            _updateUserDto.Street = "Test";
            _updateUserDto.City = "Test";
            _updateUserDto.StreetNumber = "Test1234";

            var user = new User();
            user.Email = "testemail@gmail.com";
            user.AddressId = 1;
            user.DocumentId = 1;
            user.Address = new Address
            {
                Id = 1
            };
            user.IdentificationDocument = new IdentificationDocument
            {
                Id = 1
            };
            _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(user.Email).Returns(user);
            _mapper.Map<User>(_updateUserDto).Returns(user);
            _unitOfWork.UserRepository.UpdateAndSave(user).Returns(1);

            // Act
            var validationResult = _validator.TestValidate(_updateUserDto);
            var result = await _sut.Update(_updateUserDto, "testemail@gmail.com");

            // Assert
            validationResult.ShouldNotHaveAnyValidationErrors();
            Assert.Equal("User successfully updated", result.Message);
            Assert.Equal("testemail@gmail.com", user.Email);
            Assert.True(result.Success);
        }

        [Fact]
        public void Update_EmptyName_ReturnsError()
        {
            // Arrange
            _updateUserDto.FirstName = "";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.FirstName)
                .WithErrorMessage("'First Name' must not be empty.");
        }

        [Fact]
        public void Update_FirstNameContainsInvalidCharacters_ReturnsError()
        {
            // Arrange
            _updateUserDto.FirstName = "Test1234";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.FirstName)
                .WithErrorMessage("Name should only contain letters.");
        }

        [Theory]
        [InlineData("T")]
        [InlineData("TESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTEST")]
        public void Update_FirstNameInvalidLength_ReturnsError(string firstName)
        {
            // Arrange
            _updateUserDto.FirstName = firstName;

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.FirstName);
        }

        [Fact]
        public void Update_EmptyLastName_ReturnsError()
        {
            // Arrange
            _updateUserDto.LastName = "";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.LastName)
                .WithErrorMessage("'Last Name' must not be empty.");
        }

        [Fact]
        public void Update_LastNameContainsInvalidCharacters_ReturnsError()
        {
            // Arrange
            _updateUserDto.LastName = "Test1234";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.LastName)
                .WithErrorMessage("Surname should only contain letters.");
        }

        [Theory]
        [InlineData("T")]
        [InlineData("TESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTEST")]
        public void Update_LastNameInvalidLength_ReturnsError(string lastName)
        {
            // Arrange
            _updateUserDto.LastName = lastName;

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.LastName);
        }

        [Fact]
        public void Update_EmptyPhoneNumber_ReturnsError()
        {
            // Arrange
            _updateUserDto.PhoneNumber = "";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.PhoneNumber)
                .WithErrorMessage("'Phone Number' must not be empty.");
        }

        [Fact]
        public void Update_PhoneNumberInvalidFormat_ReturnsError()
        {
            // Arrange
            _updateUserDto.PhoneNumber = "1234";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.PhoneNumber)
                .WithErrorMessage("Phone number should be in format '+1234'.");
        }

        [Fact]
        public void Update_InvalidDateOfBirthOlderThan120Years_ReturnsError()
        {
            // Arrange
            _updateUserDto.DateOfBirth = DateTime.Now.AddYears(-121);

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.DateOfBirth)
                .WithErrorMessage("Age must be in the range 16-120.");
        }

        [Fact]
        public void Update_InvalidDateOfBirthYoungerThan16Years_ReturnsError()
        {
            // Arrange
            _updateUserDto.DateOfBirth = DateTime.Now.AddYears(-15);

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.DateOfBirth)
                .WithErrorMessage("Age must be in the range 16-120.");
        }

        [Fact]
        public void Update_EmptyCity_ReturnsError()
        {
            // Arrange
            _updateUserDto.City = "";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.City)
                .WithErrorMessage("'City' must not be empty.");
        }

        [Fact]
        public void Update_CityContainsInvalidCharacters_ReturnsError()
        {
            // Arrange
            _updateUserDto.City = "Test1234";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.City)
                .WithErrorMessage("City should only contain letters.");
        }

        [Fact]
        public void Update_EmptyCountry_ReturnsError()
        {
            // Arrange
            _updateUserDto.Country = "";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.Country)
                .WithErrorMessage("'Country' must not be empty.");
        }

        [Fact]
        public void Update_CountryContainsInvalidCharacters_ReturnsError()
        {
            // Arrange
            _updateUserDto.Country = "Test1234";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.Country)
                .WithErrorMessage("Country should only contain letters.");
        }

        [Fact]
        public void Update_EmptyStreet_ReturnsError()
        {
            // Arrange
            _updateUserDto.Street = "";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.Street)
                .WithErrorMessage("'Street' must not be empty.");
        }

        [Fact]
        public void Update_StreetContainsInvalidCharacters_ReturnsError()
        {
            // Arrange
            _updateUserDto.Street = "Test1234";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.Street)
                .WithErrorMessage("Street should only contain letters.");
        }

        [Fact]
        public void Update_EmptyStreetNumber_ReturnsError()
        {
            // Arrange
            _updateUserDto.StreetNumber = "";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.StreetNumber)
                .WithErrorMessage("'Street Number' must not be empty.");
        }

        [Fact]
        public void Update_StreetNumberContainsInvalidCharacters_ReturnsError()
        {
            // Arrange
            _updateUserDto.StreetNumber = "Test1234*";

            // Act
            var result = _validator.TestValidate(_updateUserDto);

            // Assert
            result.ShouldHaveValidationErrorFor(updateUser => updateUser.StreetNumber)
                .WithErrorMessage("Street number should only contain letters and numbers.");
        }
    }
}