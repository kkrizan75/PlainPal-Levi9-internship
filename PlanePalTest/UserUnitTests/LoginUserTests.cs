using Microsoft.Extensions.Configuration;
using NSubstitute;
using PlanePal.DTOs.User;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Implementation;
using PlanePal.Services.Interfaces;
using PlanePal.Services.Shared;
using Xunit;

namespace PlanePal.UserUnitTests
{
    public class LoginUserTests
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly LoginUserDTO _loginDTO;

        public LoginUserTests()
        {
            _authenticationService = new AuthenticationService(_unitOfWork, GetConfig());
            _loginDTO = new();
        }

        [Fact]
        public async Task WrongPassword_ReturnsError()
        {
            //Arrange
            _loginDTO.Email = "test@test.com";
            _loginDTO.Password = "not_password";
            _unitOfWork.UserRepository.GetByEmail(_loginDTO).Returns(new User()
            {
                Email = _loginDTO.Email,
                Salt = HashnSaltService.GenerateSalt(),
                Password = "password"
            });

            //Act
            var result = await _authenticationService.Authenticate(_loginDTO);

            //Assert
            Assert.True(result is null);
        }

        [Fact]
        public async Task LoginHappyPath_ReturnsSuccess()
        {
            //Arrange
            var salt = HashnSaltService.GenerateSalt();
            _loginDTO.Email = "test@test.com";
            _loginDTO.Password = "password";
            _unitOfWork.UserRepository.GetByEmail(_loginDTO).Returns(new User()
            {
                Email = _loginDTO.Email,
                Salt = salt,
                Password = HashnSaltService.HashWithSalt("password", salt)
            });

            //Act
            var result = await _authenticationService.Authenticate(_loginDTO);

            //Assert
            Assert.True(result is not null);
        }

        private IConfiguration GetConfig()
        {
            var myConfiguration = new Dictionary<string, string>
            {
                {"Jwt:Key", "Value1"},
                {"Jwt:Issuer","" },
                {"Jwt:Audience","" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();
            return configuration;
        }
    }
}