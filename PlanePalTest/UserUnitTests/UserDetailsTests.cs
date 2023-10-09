using AutoMapper;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using PlanePal.DTOs.Blob;
using PlanePal.DTOs.User;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Implementation;
using PlanePal.Services.Interfaces;
using Xunit;

namespace PlanePal.UserUnitTests
{
    public class UserDetailsTests
    {
        private readonly IUserService _sut;
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IAzureStorage _azureStorage = Substitute.For<IAzureStorage>();
    
        public UserDetailsTests() 
        {
            _sut = new UserService(_unitOfWork, _mapper, _azureStorage);
        }

        [Fact]
        public async Task LoadDetails_InputIncorrectEmail_ReturnsError()
        {
            // Arange
            var email = "testemail@gmail.com";
            _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(Arg.Any<string>()).ReturnsNull();

            // Act
            var result = await _sut.LoadUserData(email);

            // Assert
            Assert.Null(result.Data);
            Assert.Equal($"User with an email {email} not found", result.Message);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task LoadDetails_HappyPath_ReturnsSuccess()
        {
            // Arrange
            var userEmail = "user@gmail.com";
            var document = new IdentificationDocument { Id = 1, DocumentImageUri = "12345" };
            var user = new User
            {
                Email = "user@gmail.com",
                DocumentId = 1,
                IdentificationDocument = document
            };
            var userDTO = new ReadUserDetailsDTO {
                Email = user.Email,
                IdentificationDocument = new DocumentDetailsDTO { }
                
            };
            var blobDTO = new BlobDTO { }; 
            _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(userEmail).Returns(user);
            _azureStorage.DownloadAsync("12345").Returns(blobDTO);
            _mapper.Map<ReadUserDetailsDTO>(user).Returns(userDTO);
            
            // Act
            var result = await _sut.LoadUserData(userEmail);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal($"User data successfully loaded", result.Message);
        }
        [Fact]
        public async Task LoadDetails_ThrowsException_ReturnsError()
        {
            // Arrange
            var userEmail = "user@gmail.com";
            var document = new IdentificationDocument { Id = 1, DocumentImageUri = "12345" };
            var user = new User
            {
                Email = "user@gmail.com",
                DocumentId = 1,
                IdentificationDocument = document
            };
            var userDTO = new ReadUserDetailsDTO
            {
                Email = user.Email,
                IdentificationDocument = new DocumentDetailsDTO { }

            };
            _unitOfWork.UserRepository.GetOneWithAddressIdAndDocumentId(userEmail).Returns(user);
            _azureStorage.DownloadAsync("12345").ThrowsAsync<Exception>();
            _mapper.Map<ReadUserDetailsDTO>(user).Returns(userDTO);

            // Act
            var result = await _sut.LoadUserData(userEmail);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.NotNull(result.Message);
        }
    }
}
