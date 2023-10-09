using AutoMapper;
using FluentValidation.TestHelper;
using MimeKit.Cryptography;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PlanePal.DTOs.User;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Implementation;
using PlanePal.Services.Interfaces;
using PlanePal.Validators;
using Xunit;

namespace PlanePal.UserUnitTests
{
    public class CreateUserTests
    {
        private readonly IUserService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IAzureStorage _azureStorage = Substitute.For<IAzureStorage>();
        private readonly CreateUserValidator _validator;
        private readonly CreateUserDTO _createUserDTO;

        public CreateUserTests()
        {
            _sut = new UserService(_unitOfWork, _mapper, _azureStorage);
            _validator = new();
            _createUserDTO = new();
            _createUserDTO.Address = new AddressDTO();
            _createUserDTO.IdentificationDocument = new DocumentDTO();
        }

        [Fact]
        public void Create_InputIncorrectEmail_ReturnsError()
        {
            // Arange
            _createUserDTO.Email = "testemail-gmail.com";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.Email)
                .WithErrorMessage("A valid email is required.");
        }

        [Fact]
        public void Create_EmptyEmail_ReturnsError()
        {
            // Arrange
            _createUserDTO.Email = "";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.Email)
                .WithErrorMessage("'Email' must not be empty.");
        }

        [Fact]
        public void Create_InputIncorrectFirstName_ReturnsError()
        {
            // Arange
            _createUserDTO.FirstName = "367Test";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.FirstName)
                .WithErrorMessage("Name should only contain letters.");
        }

        [Fact]
        public void Create_EmptyFirstName_ReturnsError()
        {
            // Arrange
            _createUserDTO.FirstName = "";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.FirstName)
                .WithErrorMessage("'First Name' must not be empty.");
        }

        [Fact]
        public void Create_InputIncorrectLastName_ReturnsError()
        {
            // Arange
            _createUserDTO.LastName = "367Test";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.LastName)
                .WithErrorMessage("Surname should only contain letters.");
        }

        [Fact]
        public void Create_EmptyLastName_ReturnsError()
        {
            // Arrange
            _createUserDTO.LastName = "";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.LastName)
                .WithErrorMessage("'Last Name' must not be empty.");
        }

        [Fact]
        public void Create_InputIncorrectPassword_ReturnsError()
        {
            // Arange
            _createUserDTO.Password = "367testtttt";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.Password)
                .WithErrorMessage("Password must contain at least one upper case letter, one lower case letter, one number and one special character.");
        }

        [Fact]
        public void Create_InputIncorrectPasswordLength_ReturnsError()
        {
            // Arange
            _createUserDTO.Password = "367Tes!";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.Password)
                .WithErrorMessage("Password must contain at least 10 characters");
        }

        [Fact]
        public void Create_EmptyPassword_ReturnsError()
        {
            // Arrange
            _createUserDTO.Password = "";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.Password)
                .WithErrorMessage("'Password' must not be empty.");
        }

        [Fact]
        public void Create_InputIncorrectPhoneNumber_ReturnsError()
        {
            // Arange
            _createUserDTO.PhoneNumber = "367Tes!";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.PhoneNumber)
                .WithErrorMessage("Phone number should be in format '+1234'.");
        }

        [Fact]
        public void Create_EmptyPhoneNumber_ReturnsError()
        {
            // Arrange
            _createUserDTO.PhoneNumber = "";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.PhoneNumber)
                .WithErrorMessage("'Phone Number' must not be empty.");
        }

        [Fact]
        public void Create_InvalidDateOfBirthOlderThan120Years_ReturnsError()
        {
            // Arrange
            _createUserDTO.DateOfBirth = DateTime.Now.AddYears(-121);

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.DateOfBirth)
                .WithErrorMessage("Age must be in the range 16-120.");
        }

        [Fact]
        public void Create_InvalidDateOfBirthYoungerThan16Years_ReturnsError()
        {
            // Arrange
            _createUserDTO.DateOfBirth = DateTime.Now.AddYears(-15);

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.DateOfBirth)
                .WithErrorMessage("Age must be in the range 16-120.");
        }

        [Fact]
        public void Create_EmptyCity_ReturnsError()
        {
            // Arrange
            _createUserDTO.Address.City = "";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.Address.City)
                .WithErrorMessage("'Address City' must not be empty.");
        }

        [Fact]
        public void Create_CityContainsInvalidCharacters_ReturnsError()
        {
            // Arrange
            _createUserDTO.Address.City = "Test1234";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.Address.City)
                .WithErrorMessage("City should only contain letters.");
        }

        [Fact]
        public void Create_EmptyCountry_ReturnsError()
        {
            // Arrange
            _createUserDTO.Address.Country = "";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.Address.Country)
                .WithErrorMessage("'Address Country' must not be empty.");
        }

        [Fact]
        public void Create_CountryContainsInvalidCharacters_ReturnsError()
        {
            // Arrange
            _createUserDTO.Address.Country = "Test1234";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.Address.Country)
                .WithErrorMessage("Country should only contain letters.");
        }

        [Fact]
        public void Create_EmptyStreet_ReturnsError()
        {
            // Arrange
            _createUserDTO.Address.Street = "";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.Address.Street)
                .WithErrorMessage("'Address Street' must not be empty.");
        }

        [Fact]
        public void Create_StreetContainsInvalidCharacters_ReturnsError()
        {
            // Arrange
            _createUserDTO.Address.Street = "Test1234";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.Address.Street)
                .WithErrorMessage("Street should only contain letters.");
        }

        [Fact]
        public void Create_EmptyStreetNumber_ReturnsError()
        {
            // Arrange
            _createUserDTO.Address.StreetNumber = "";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.Address.StreetNumber)
                .WithErrorMessage("'Address Street Number' must not be empty.");
        }

        [Fact]
        public void Create_StreetNumberContainsInvalidCharacters_ReturnsError()
        {
            // Arrange
            _createUserDTO.Address.StreetNumber = "Test1234*";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.Address.StreetNumber)
                .WithErrorMessage("Street number should only contain letters and numbers.");
        }

        [Fact]
        public void Create_DocumentNumberContainsInvalidCharacters_ReturnsError()
        {
            // Arrange
            _createUserDTO.IdentificationDocument.DocumentNumber = "Test123!!*";

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.IdentificationDocument.DocumentNumber)
                .WithErrorMessage("Document number must include only letters and numbers.");
        }

        [Fact]
        public void Create_DocumentTypeInvalidEnum_ReturnsError()
        {
            // Arrange
            _createUserDTO.IdentificationDocument.DocumentType = (Enums.IdDocumentTypeEnum)7;

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.IdentificationDocument.DocumentType)
                .WithErrorMessage("Document type does not exist.");
        }

        [Fact]
        public void Create_DocumentExpirationDateInvalid_ReturnsError()
        {
            // Arrange
            _createUserDTO.IdentificationDocument.ExpirationDate = DateTime.Now;

            // Act
            var result = _validator.TestValidate(_createUserDTO);

            // Assert
            result.ShouldHaveValidationErrorFor(createUser => createUser.IdentificationDocument.ExpirationDate)
                .WithErrorMessage("Document cannot expire in less than 3 months.");
        }

        //test servisa
        [Fact]
        public async Task Create_InputAlreadyExitingEmail_ReturnsError()
        {
            // Arange
            var user = new User();
            user.Email = "email@gmail.com";
            _createUserDTO.Email = "email@gmail.com";
            _unitOfWork.UserRepository.GetOne(Arg.Any<string>()).Returns(user);
            // Act
            var result = await _sut.Create(_createUserDTO);

            // Assert
            Assert.Null(result.Data);
            Assert.Equal($"User with an email {_createUserDTO.Email} already exists", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task Create_HappyPath_ReturnsSuccess()
        {
            // Arange
            _createUserDTO.Email = "string@string.com";
            _createUserDTO.FirstName = "String";
            _createUserDTO.LastName = "String";
            _createUserDTO.PhoneNumber = "+65748";
            _createUserDTO.DateOfBirth = DateTime.Now.AddYears(-20);
            _createUserDTO.Password = "Dobrasifra1!";
            _createUserDTO.Address.City = "Novi Sad";
            _createUserDTO.Address.Country = "Serbia";
            _createUserDTO.Address.Street = "Zmaj Jovina";
            _createUserDTO.Address.StreetNumber = "24a";
            _createUserDTO.IdentificationDocument = null;

            _unitOfWork.UserRoleRepository.GetOne(2).Returns(new UserRole() { Id = 2, Name = "CLIENT" });
            var user = new User { Password = "Password!@" };
            _mapper.Map<User>(_createUserDTO).Returns(user);
            _unitOfWork.UserRepository.Create(user).Returns(1);

            // Act
            var result = await _sut.Create(_createUserDTO);

            // Assert
            Assert.Null(result.Data);
            Assert.Equal($"User with an email {_createUserDTO.Email} successfully created.", result.Message);
            Assert.True(result.Success);
        }
        [Fact]
        public async Task Create_InvalidCreate_ReturnsErrror()
        {
            // Arange
            _createUserDTO.Email = "string@string.com";
            _createUserDTO.FirstName = "String";
            _createUserDTO.LastName = "String";
            _createUserDTO.PhoneNumber = "+65748";
            _createUserDTO.DateOfBirth = DateTime.Now.AddYears(-20);
            _createUserDTO.Password = "Dobrasifra1!";
            _createUserDTO.Address.City = "Novi Sad";
            _createUserDTO.Address.Country = "Serbia";
            _createUserDTO.Address.Street = "Zmaj Jovina";
            _createUserDTO.Address.StreetNumber = "24a";
            _createUserDTO.IdentificationDocument = null;

            _unitOfWork.UserRoleRepository.GetOne(2).Returns(new UserRole() { Id = 2, Name = "CLIENT" });
            var user = new User { Password = "Password!@" };
            _mapper.Map<User>(_createUserDTO).Returns(user);
            _unitOfWork.UserRepository.Create(user).Returns(0);

            // Act
            var result = await _sut.Create(_createUserDTO);

            // Assert
            Assert.Null(result.Data);
            Assert.Equal($"User with an email {_createUserDTO.Email} not created.", result.Message);
            Assert.False(result.Success);
        }
        [Fact]
        public async Task Create_CreateThrowsException_ReturnsError()
        {
            // Arange
            _createUserDTO.Email = "string@string.com";
            _createUserDTO.FirstName = "String";
            _createUserDTO.LastName = "String";
            _createUserDTO.PhoneNumber = "+65748";
            _createUserDTO.DateOfBirth = DateTime.Now.AddYears(-20);
            _createUserDTO.Password = "Dobrasifra1!";
            _createUserDTO.Address.City = "Novi Sad";
            _createUserDTO.Address.Country = "Serbia";
            _createUserDTO.Address.Street = "Zmaj Jovina";
            _createUserDTO.Address.StreetNumber = "24a";
            _createUserDTO.IdentificationDocument = null;

            _unitOfWork.UserRoleRepository.GetOne(2).Returns(new UserRole() { Id = 2, Name = "CLIENT" });
            var user = new User { Password = "Password!@" };
            _mapper.Map<User>(_createUserDTO).Returns(user);
            _unitOfWork.UserRepository.Create(user).ThrowsAsync<Exception>();

            // Act
            var result = await _sut.Create(_createUserDTO);

            // Assert
            Assert.Null(result.Data);
            Assert.NotNull(result.Message);
            Assert.False(result.Success);
        }
    }
}