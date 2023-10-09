using AutoMapper;
using FluentValidation.TestHelper;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using PlanePal.DTOs.User;
using PlanePal.Enums;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Implementation;
using PlanePal.Services.Interfaces;
using PlanePal.Validators;
using Xunit;

namespace PlanePal.UserUnitTests
{
    public class UpdateDocumentTests
    {
        private readonly IUserService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IAzureStorage _azureStorage = Substitute.For<IAzureStorage>();
        private readonly DocumentDTO _documentDto;
        private readonly CreateUserDTO _createUserDTO;
        private readonly IdentificationDocumentValidator _validator;

        public UpdateDocumentTests()
        {
            _sut = new UserService(_unitOfWork, _mapper, _azureStorage);
            _validator = new();
            _documentDto = new();
            _createUserDTO = new()
            {
                Address = new AddressDTO()
            };
        }

        [Fact]
        public void Validity_HappyPath_ReturnsSuccess()
        {
            //Arrange
            _documentDto.DocumentNumber = "abcd123";
            _documentDto.DocumentType = IdDocumentTypeEnum.PASSPORT;
            _documentDto.ExpirationDate = DateTime.Now.AddMonths(4);

            //Act
            var validationResult = _validator.TestValidate(_documentDto);

            //Assert
            validationResult.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Update_HappyPath_ReturnsSuccess()
        {
            //Arrange
            var email = "test@example.com";
            var user = new User() { Email = email };
            _documentDto.DocumentNumber = "abcd123";
            _documentDto.DocumentType = IdDocumentTypeEnum.PASSPORT;
            _documentDto.ExpirationDate = DateTime.Now.AddMonths(4);
            var document = new IdentificationDocument()
            {
                Id = 1,
                DocumentNumber = _documentDto.DocumentNumber,
                DocumentType = _documentDto.DocumentType,
                ExpirationDate = _documentDto.ExpirationDate
            };
            _mapper.Map<IdentificationDocument>(_documentDto).Returns(document);

            _unitOfWork.UserRepository.GetOne(email).Returns(user);
            _unitOfWork.DocumentRepository.Create(document).Returns(1);
            _unitOfWork.UserRepository.UpdateAndSave(user).Returns(1);

            // Act
            var result = await _sut.UpdateDocument("test@example.com", _documentDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Successfully updated document.", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task UpdateDocument_UserNotFound_ReturnsErrorResponse()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _unitOfWork.UserRepository.GetOne(email).ReturnsNull();

            // Act
            var result = await _sut.UpdateDocument(email, _documentDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal($"User with an email {email} not found", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task UpdateDocument_ExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var email = "test@example.com";
            _documentDto.DocumentNumber = "abcd123";
            _documentDto.DocumentType = IdDocumentTypeEnum.PASSPORT;
            _documentDto.ExpirationDate = DateTime.Now.AddMonths(4);
            _unitOfWork.UserRepository.GetOne(email).Returns(new User(){ Email=email});
            _unitOfWork.DocumentRepository.Create(new IdentificationDocument()).ThrowsAsync<Exception>();

            // Act
            var result = await _sut.UpdateDocument("test@example.com", _documentDto);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task UpdateDocument_CreateDocumentFailed_ReturnsErrorResponse()
        {
            // Arrange
            var email = "test@example.com";
            _documentDto.DocumentNumber = "abcd123";
            _documentDto.DocumentType = IdDocumentTypeEnum.PASSPORT;
            _documentDto.ExpirationDate = DateTime.Now.AddMonths(4);
            var document= new IdentificationDocument()
            {
                Id = 1,
                DocumentNumber = _documentDto.DocumentNumber,
                DocumentType = _documentDto.DocumentType,
                ExpirationDate = _documentDto.ExpirationDate
            };
            _mapper.Map<IdentificationDocument>(_documentDto).Returns(document);
            
            _unitOfWork.UserRepository.GetOne(email).Returns(new User() { Email = email });
            _unitOfWork.DocumentRepository.Create(document).Returns(0);

            // Act
            var result = await _sut.UpdateDocument("test@example.com", _documentDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Unable to update user document.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task UpdateDocument_UpdateAndSaveFailed_ReturnsErrorResponse()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User() { Email = email };
            _documentDto.DocumentNumber = "abcd123";
            _documentDto.DocumentType = IdDocumentTypeEnum.PASSPORT;
            _documentDto.ExpirationDate = DateTime.Now.AddMonths(4);
            var document = new IdentificationDocument()
            {
                Id = 1,
                DocumentNumber = _documentDto.DocumentNumber,
                DocumentType = _documentDto.DocumentType,
                ExpirationDate = _documentDto.ExpirationDate
            };
            _mapper.Map<IdentificationDocument>(_documentDto).Returns(document);

            _unitOfWork.UserRepository.GetOne(email).Returns(user);
            _unitOfWork.DocumentRepository.Create(document).Returns(1);
            _unitOfWork.UserRepository.UpdateAndSave(user).Returns(0);

            // Act
            var result = await _sut.UpdateDocument("test@example.com", _documentDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Unable to update user document.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public void Update_EmptyDocumentNumber_ReturnsError()
        {
            // Arrange
            _documentDto.DocumentNumber = "";

            // Act
            var result = _validator.TestValidate(_documentDto);

            // Assert
            result.ShouldHaveValidationErrorFor(_document => _document.DocumentNumber)
                .WithErrorMessage("'Document Number' must not be empty.");
        }

        [Fact]
        public void Update_DocumentNumberContainsInvalidCharacters_ReturnsError()
        {
            // Arrange
            _documentDto.DocumentNumber = "Test1234*";

            // Act
            var result = _validator.TestValidate(_documentDto);

            // Assert
            result.ShouldHaveValidationErrorFor(_document => _document.DocumentNumber)
                .WithErrorMessage("Document number must include only letters and numbers.");
        }

        [Fact]
        public void Update_InvalidExpirationDateExpiresInLessThan3Months_ReturnsError()
        {
            // Arrange
            _documentDto.ExpirationDate = DateTime.Now.AddMonths(2);

            // Act
            var result = _validator.TestValidate(_documentDto);

            // Assert
            result.ShouldHaveValidationErrorFor(_document => _document.ExpirationDate)
                .WithErrorMessage("Document cannot expire in less than 3 months.");
        }
    }
}